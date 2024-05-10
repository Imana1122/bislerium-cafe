using Application.DTO.Response.Blogs;
using Application.Service.Blogs.Queries.BlogImages;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogImages
{
    //Handle get all blog images
    public class GetAllBlogImagesHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetAllBlogImagesQuery, IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(GetAllBlogImagesQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the database context using the factory
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve all blog images from the database, projecting only the Image property
            var images = await dbContext.BlogImages
                .Select(image => image.Image) // Project only the Image property
                .AsNoTracking() // Ensure entities are read-only to improve performance
                .ToListAsync(cancellationToken); // Materialize the query into a list asynchronously

            // Return the list of image URLs
            return images;
        }




    }
}
