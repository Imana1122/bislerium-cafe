using Application.DTO.Response.Blogs;
using Application.Extensions.Identity;
using Application.Service.Blogs.Queries.BlogComments;
using Application.Service.Blogs.Queries.BlogImages;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogImages
{
    //Handler to get all blog images of a blog by blog id
    public class GetBlogImagesByBlogIdHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetBlogImagesByBlogIdQuery, IEnumerable<BlogImage>>
    {
        public async Task<IEnumerable<BlogImage>> Handle(GetBlogImagesByBlogIdQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the database context using the factory
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve blog images associated with the specified blog ID
            var images = await dbContext.BlogImages
                .Where(comment => comment.BlogId.Equals(request.Id)) // Filter by the provided blog ID
                .AsNoTracking() // Ensure entities are read-only to improve performance
                .ToListAsync(cancellationToken); // Materialize the query into a list asynchronously

            // Create a list to hold the response DTOs
            var responseDTOs = new List<BlogImage>();

            // Iterate through each retrieved image entity
            foreach (var image in images)
            {
                // Map each image entity to a DTO
                var dataDTO = image.Adapt<BlogImage>(); // Assuming AutoMapper is used for mapping
                responseDTOs.Add(dataDTO); // Add the mapped DTO to the list
            }

            // Return the list of response DTOs
            return responseDTOs;
        }



    }
}
