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
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    //Handler to get all blogs with all reaction and comments count with reaction status if user id is not null
    public class GetAllBlogsWithReactionStatusHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<GetAllBlogsWithReactionStatusQuery, IEnumerable<GetBlogsResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogsResponseDTO>> Handle(GetAllBlogsWithReactionStatusQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the DbContext using the context factory
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve all blogs from the database, including related images, reactions, and comments
            var data = await dbContext.Blogs
                .Include(blog => blog.Images) // Include related images
                .Include(blog => blog.Reactions) // Include related reactions
                .Include(blog => blog.Comments) // Include related comments
                .AsNoTracking() // Disable change tracking for performance
                .ToListAsync(cancellationToken); // Execute the query asynchronously

            // Adapt each blog entity to GetBlogsResponseDTO
            var responseDTOs = data.Select(blog =>
            {
                // Adapt the current blog entity to a GetBlogsResponseDTO
                var blogResponseDTO = blog.Adapt<GetBlogsResponseDTO>();

                // Map images to DTOs
                blogResponseDTO.Images = blog.Images?.Select(image => image.Adapt<GetBlogImageResponseDTO>()).ToList();

                // Calculate upvote count
                blogResponseDTO.UpvoteCount = blog.Reactions?.Count(r => r.IsUpvote) ?? 0;

                // Calculate downvote count
                blogResponseDTO.DownvoteCount = blog.Reactions?.Count(r => !r.IsUpvote) ?? 0;

                // Set comments count
                blogResponseDTO.CommentsCount = blog.Comments?.Count ?? 0;

                // Calculate PopularityCount for a BlogResponseDTO
                blogResponseDTO.PopularityCount = CalculatePopularityCount(blogResponseDTO);

                // Check if a user is authenticated
                if (request.Id != null)
                {
                    // Retrieve the reaction of the current user for this blog
                    BlogReaction reaction = blog.Reactions
                        .FirstOrDefault(_ => _.UserId.Equals(request.Id) && _.BlogId.Equals(blog.Id));

                    // Set upvoted and downvoted status based on the user's reaction
                    blogResponseDTO.UpvotedStatus = reaction?.IsUpvote == true;
                    blogResponseDTO.DownvotedStatus = reaction?.IsDownvote == true;
                }
                else
                {
                    // If the user is not authenticated, set both upvoted and downvoted status to false
                    blogResponseDTO.UpvotedStatus = false;
                    blogResponseDTO.DownvotedStatus = false;
                }

                return blogResponseDTO;
            }).ToList(); // Convert the IEnumerable to a List

            // Iterate over each response DTO to fetch and set the blogger name
            foreach (var item in responseDTOs)
            {
                // Find the user by ID asynchronously
                var user = await userManager.FindByIdAsync(item.UserId.ToString());

                if (user != null)
                {
                    // If the user is found, set the blogger name
                    item.BloggerName = user.Name;
                }
                else
                {
                    // If the user is not found, set the blogger name to "anonymous"
                    item.BloggerName = "anonymous";
                }
            }

            // Return the list of response DTOs
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
