using Application.DTO.Response;
using Application.Service.Blogs.Commands.Blogs;
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

namespace Infrastructure.Repository.Products.Handlers.Blogs
{
    // Handler class responsible for creating a new blog

    public class CreateBlogHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<CreateBlogCommand, ServiceResponse>
    {
        // Method to handle the creation of a new blog
        public async Task<ServiceResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Mapping the DTO to the domain entity Blog
                var data = request.BlogModel.Adapt(new Blog());

                // Adding the new blog entity to the context
                dbContext.Blogs.Add(data);
              

                // Creating user history for the blog creation
                var history = new History
                {
                    UserId = request.BlogModel.UserId,
                    Content = request.BlogModel.Title + " is created."
                };

                // Adding user history to the context
                dbContext.Histories.Add(history);

                // Saving changes to the database
                await dbContext.SaveChangesAsync(cancellationToken);

                // Returning success response with created blog title
                return GeneralDbResponses.ItemCreated(request.BlogModel.Title);
            }
            catch (Exception ex)
            {
                // Returning error response if an exception occurs
                return new ServiceResponse(true, ex.Message);
            }
        }
    }
}
