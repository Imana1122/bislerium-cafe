using Application.DTO.Response.Blogs;
using Application.Extensions.Identity;
using Application.Service.Blogs.Queries.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.Blogs
{
    //Handler to get blogs by popularity in Month
    public class GetBlogsWithPopularityByMonthHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<GetBlogsWithPopularityByMonthQuery, IEnumerable<GetBlogsResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogsResponseDTO>> Handle(GetBlogsWithPopularityByMonthQuery request, CancellationToken cancellationToken)
        {
            // Using statement to ensure proper disposal of DbContext
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve all blogs from the database, including related entities
            var data = await dbContext.Blogs
                .Include(blog => blog.Images) // Include related images
                .Include(blog => blog.Reactions) // Include related reactions
                .Include(blog => blog.Comments) // Include related comments
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Adapt each blog entity to GetBlogsResponseDTO
            var responseDTOs = data.Select(blog =>
            {
                var blogResponseDTO = blog.Adapt<GetBlogsResponseDTO>();

                // Map images to DTOs
                blogResponseDTO.Images = blog.Images?.Select(image => image.Adapt<GetBlogImageResponseDTO>()).ToList();

                // Calculate upvote count for the specified month
                blogResponseDTO.UpvoteCount = blog.Reactions?.Where(_ => _.CreatedAt.Month == request.month).Count(r => r.IsUpvote) ?? 0;

                // Calculate downvote count for the specified month
                blogResponseDTO.DownvoteCount = blog.Reactions?.Where(_ => _.CreatedAt.Month == request.month).Count(r => !r.IsUpvote) ?? 0;

                // Set comments count for the specified month
                blogResponseDTO.CommentsCount = blog.Comments?.Count(comment => comment.CreatedAt.Month == request.month) ?? 0;

                // Calculate PopularityCount for a BlogResponseDTO
                blogResponseDTO.PopularityCount = CalculatePopularityCount(blogResponseDTO);

                // Initialize upvoted and downvoted status as false
                blogResponseDTO.UpvotedStatus = false;
                blogResponseDTO.DownvotedStatus = false;

                return blogResponseDTO;
            });

            // Convert responseDTOs to a list to iterate through it
            responseDTOs = responseDTOs.ToList();

            // Iterate through each responseDTO and set the blogger name
            foreach (var item in responseDTOs)
            {
                // Find the user by ID using the user manager
                var user = await userManager.FindByIdAsync(item.UserId.ToString());

                // If user found, set the blogger name, otherwise set it as "anonymous"
                if (user != null)
                {
                    item.BloggerName = user.Name;
                }
                else
                {
                    item.BloggerName = "anonymous";
                }
            }

            return responseDTOs;
        }


        // Calculate PopularityCount for a BlogResponseDTO
        int CalculatePopularityCount(GetBlogsResponseDTO blog)
        {
            // Retrieve upvote count, downvote count, and comments count from the blog
            int upvoteCount = blog.UpvoteCount;
            int downvoteCount = blog.DownvoteCount;
            int commentsCount = blog.CommentsCount;

            // Define weightage rates
            int upvoteWeightage = 2;
            int downvoteWeightage = -1;
            int commentWeightage = 1;

            // Calculate PopularityCount using the formula
            int popularityCount = upvoteCount * upvoteWeightage + downvoteCount * downvoteWeightage + commentsCount * commentWeightage;

            return popularityCount;
        }


    }
}
