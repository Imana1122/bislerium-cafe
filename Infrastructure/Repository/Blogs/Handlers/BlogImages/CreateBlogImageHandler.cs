using Application.DTO.Request.Blogs;
using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.BlogImages;
using Domain.Entities;
using Infrastructure.DataAccess;
using Infrastructure.Repository.Products;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogImages
{
    //Handler to create new BlogImages for a blog
    public class CreateBlogImageHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<CreateBlogImageCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateBlogImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Retrieve the blog from the database by ID
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.imageModel.BlogId), cancellationToken);

                // If the blog with the specified ID is not found, return item not found response
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Adapt the image model to the domain entity BlogImage
                var imageData = request.imageModel.Adapt(new BlogImage());

                // Add the image data to the BlogImages table
                dbContext.BlogImages.Add(imageData);

                // Save changes to the database
                await dbContext.SaveChangesAsync(cancellationToken);

                // Return item created response
                return GeneralDbResponses.ItemCreated("Image");
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs
                return new ServiceResponse(true, ex.Message);
            }
        }

    }
}
