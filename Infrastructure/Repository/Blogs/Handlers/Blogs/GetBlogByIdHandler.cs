﻿using Application.DTO.Response.Blogs;
using Application.Service.Blogs.Queries.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.Blogs
{
    public class GetBlogByIdHandler(DataAccess.IDbContextFactory<AppDbContext> _contextFactory) : IRequestHandler<GetBlogByIdCommand, GetBlogsResponseDTO>
    {
        public async Task<GetBlogsResponseDTO> Handle(GetBlogByIdCommand request, CancellationToken cancellationToken)
        {
            using var dbContext = _contextFactory.CreateDbContext();
            var blog = await dbContext.Blogs
                .Where(b => b.Id.Equals( request.Id))
                .Include(b => b.Images) // Include related images
                .Include(b => b.Reactions) // Include related reactions
                .Include(b => b.Comments) // Include related comments
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (blog == null)
            {
                // Handle the case when the blog with the specified ID is not found
                return null;
            }

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
            if (request.UserId != null)
            {
                BlogReaction reaction = blog.Reactions
                    .Where(_ => _.UserId.Equals(request.UserId) && _.BlogId.Equals(blog.Id))
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
            }



            return blogResponseDTO;
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