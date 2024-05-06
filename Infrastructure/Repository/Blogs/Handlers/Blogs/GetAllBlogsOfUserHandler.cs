﻿using Application.DTO.Response.Blogs;
using Application.Extensions.Identity;
using Application.Service.Blogs.Queries.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
using Infrastructure.Repository.Products;
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
    public class GetAllBlogsOfUserHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<GetAllBlogsOfUserQuery, IEnumerable<GetBlogsResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogsResponseDTO>> Handle(GetAllBlogsOfUserQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                // If user not found, return an empty collection
                return Enumerable.Empty<GetBlogsResponseDTO>();
            }
            using var dbContext = contextFactory.CreateDbContext();


            var data = await dbContext.Blogs
                .Where(_=>_.UserId.Equals(request.Id))
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

                // Calculate upvote count
                blogResponseDTO.UpvoteCount = blog.Reactions?.Count(r => r.IsUpvote) ?? 0;

                // Calculate downvote count
                blogResponseDTO.DownvoteCount = blog.Reactions?.Count(r => !r.IsUpvote) ?? 0;

                // Set comments count
                blogResponseDTO.CommentsCount = blog.Comments?.Count ?? 0;
                // Calculate PopularityCount for a BlogResponseDTO
                blogResponseDTO.PopularityCount = CalculatePopularityCount(blogResponseDTO);

                BlogReaction reaction = blog.Reactions
                        .Where(_ => _.UserId.Equals(request.Id) && _.BlogId.Equals(blog.Id))
                        .FirstOrDefault();

                    if (reaction != null)
                    {
                        blogResponseDTO.DownvotedStatus = reaction.IsDownvote == true;
                        blogResponseDTO.UpvotedStatus = reaction.IsUpvote == true;
                    }
                    else
                    {
                        // Handle the case when no matching BlogReaction is found
                        blogResponseDTO.UpvotedStatus = false;
                        blogResponseDTO.DownvotedStatus = false;
                    }
                





                return blogResponseDTO;
            });

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
