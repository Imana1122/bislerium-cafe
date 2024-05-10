using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.BlogReactions;
using Domain.Entities;
using Infrastructure.DataAccess;
using Infrastructure.Repository.Products;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogReactions
{
    //Handler to delete blog reaction 
    public class DeleteBlogReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteBlogReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Retrieve the blog from the database by ID
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogId), cancellationToken);

                // If the blog with the specified ID is not found, return item not found response
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Find the user by ID using the user manager
                var user = await userManager.FindByIdAsync(request.UserId.ToString());

                // If the user is not found, return item not found response
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                // Find the blog reaction entry for the specified user and blog
                var reaction = await dbContext.BlogReactions.FirstOrDefaultAsync(_ => _.UserId.Equals(request.UserId) && _.BlogId == request.BlogId, cancellationToken);

                // If the blog reaction is not found, return item not found response
                if (reaction == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog reaction");
                }

                // Remove the blog reaction from the database
                dbContext.BlogReactions.Remove(reaction);

                // Create a notification for the blog owner indicating the reaction has been unreacted
                var notification = new Notification
                {
                    Read = false,
                    UserId = blog.UserId,
                    Content = blog.Title + " is unreacted by " + user.Name
                };
                dbContext.Notifications.Add(notification);

                // Save changes to the database
                await dbContext.SaveChangesAsync(cancellationToken);

                // Return item delete response
                return GeneralDbResponses.ItemDelete("Reaction");
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs
                return new ServiceResponse(true, ex.Message);
            }
        }

    }
}
