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
    public class GetBlogsWithPopularityByMonthHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<GetBlogsWithPopularityByMonthQuery, IEnumerable<GetBlogsResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogsResponseDTO>> Handle(GetBlogsWithPopularityByMonthQuery request, CancellationToken cancellationToken)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var data = await dbContext.Blogs
                .Include(blog => blog.Images) // Include related images
                .Include(blog => blog.Reactions) // Include related reactions
                .Include(blog => blog.Comments) // Include related comments
                .AsNoTracking()
                .ToListAsync(cancellationToken);


            // Adapt each blog entity to GetBlogsResponseDTO
            var responseDTOs = data.Select( blog =>
            {
                var blogResponseDTO = blog.Adapt<GetBlogsResponseDTO>();

                // Map images to DTOs
                blogResponseDTO.Images = blog.Images?.Select(image => image.Adapt<GetBlogImageResponseDTO>()).ToList();

                // Calculate upvote count
                blogResponseDTO.UpvoteCount = blog.Reactions?.Where(_=>_.CreatedAt.Month == request.month).Count(r => r.IsUpvote) ?? 0;

                // Calculate downvote count
                blogResponseDTO.DownvoteCount = blog.Reactions?.Where(_ => _.CreatedAt.Month == request.month).Count(r => !r.IsUpvote) ?? 0;

                // Set comments count
                blogResponseDTO.CommentsCount = blog.Comments?.Count(comment => comment.CreatedAt.Month == request.month) ?? 0;

                // Calculate PopularityCount for a BlogResponseDTO
                blogResponseDTO.PopularityCount = CalculatePopularityCount(blogResponseDTO);
                

                blogResponseDTO.UpvotedStatus = false;
                blogResponseDTO.DownvotedStatus = false;
               

                return blogResponseDTO;
            });

            responseDTOs = responseDTOs.ToList();
            foreach (var item in responseDTOs)
            {
                var user = await userManager.FindByIdAsync(item.UserId.ToString());
                if (user != null)
                {
                    Console.WriteLine("hello");
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
