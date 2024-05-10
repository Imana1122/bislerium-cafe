using Application.DTO.Response;
using Application.Service.Blogs.Commands.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    public class UpdateBlogHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) :IRequestHandler<UpdateBlogCommand,ServiceResponse>
    {
        //Handler to update blog 
        
        public async Task<ServiceResponse> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Retrieve the blog from the database by ID
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogModel.Id), cancellationToken: cancellationToken);

                // If the blog with the specified ID is not found, return item not found response
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound(request.BlogModel.Title);
                }
                blog.UpdatedAt=DateTime.Now;

                // Detach the existing blog entity from the context
                dbContext.Entry(blog).State = EntityState.Detached;

                // Adapt the updated blog model to the domain entity Blog
                var adaptedBlog = request.BlogModel.Adapt(new Blog());

                // Update the blog entity in the context
                dbContext.Blogs.Update(adaptedBlog);

                // Create a user history for the blog update
                var history = new History
                {
                    UserId = blog.UserId,
                    Content = blog.Title + " is updated."
                };

                // Add the user history to the context
                dbContext.Histories.Add(history);

                // Save changes to the database
                await dbContext.SaveChangesAsync(cancellationToken);

                // Return item update response
                return GeneralDbResponses.ItemUpdate(request.BlogModel.Title);
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs
                return new ServiceResponse(true, ex.Message);
            }
        }


    }
}
