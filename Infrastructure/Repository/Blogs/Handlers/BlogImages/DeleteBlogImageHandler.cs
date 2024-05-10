using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.BlogImages;
using Infrastructure.DataAccess;
using Infrastructure.Repository.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogImages
{
    //Handle delete blog image by image id
    public class DeleteBlogImageHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<DeleteBlogImageCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create a new instance of the database context using the factory
                using var dbContext = contextFactory.CreateDbContext();

                // Find the blog image by its ID
                var data = await dbContext.BlogImages.FirstOrDefaultAsync(_ => _.Id.Equals(request.Id), cancellationToken: cancellationToken);

                // If no blog image found with the provided ID, return a response indicating it wasn't found
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog Image");
                }

                // Remove the blog image from the database
                dbContext.BlogImages.Remove(data);

                // Save changes to the database
                await dbContext.SaveChangesAsync(cancellationToken);

                // Return a response indicating successful deletion of the blog image
                return GeneralDbResponses.ItemDelete("Image");
            }
            catch (Exception ex)
            {
                // If an exception occurs during the execution, return a response with an error message
                return new ServiceResponse(true, ex.Message);
            }
        }

    }
}
