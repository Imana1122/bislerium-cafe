using Application.DTO.Response.Blogs;
using Application.Service.Blogs.Queries.Blogs;
using Infrastructure.DataAccess.Blogs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    public class GetAllBlogsHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetAllBlogsQuery, IEnumerable<GetBlogsResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogsResponseDTO>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
        {
            using var dbContext = contextFactory.CreateDbContext();
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

                // Calculate upvote count
                blogResponseDTO.UpvoteCount = new GetBlogUpvoteCountResponseDTO(blog.Reactions?.Count(r => r.IsUpvote) ?? 0);

                // Calculate downvote count
                blogResponseDTO.DownvoteCount = new GetBlogDownvoteCountResponseDTO(blog.Reactions?.Count(r => !r.IsUpvote) ?? 0);

                // Set comments count
                blogResponseDTO.CommentsCount = new GetBlogCommentsCountResponseDTO(blog.Comments?.Count ?? 0);

                return blogResponseDTO;
            });

            return responseDTOs;
        }


    }
}
