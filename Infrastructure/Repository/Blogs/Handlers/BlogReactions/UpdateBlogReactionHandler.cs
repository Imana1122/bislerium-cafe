using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.BlogReactions;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogReactions
{
    //Handler to update or create blog reaction
    public class UpdateBlogReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateBlogReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Find the user by ID using the user manager
                var user = await userManager.FindByIdAsync(request.BlogReactionModel.UserId.ToString());

                // If the user is not found, return item not found response
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                // Retrieve the blog from the database by ID
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogReactionModel.BlogId), cancellationToken);

                // If the blog with the specified ID is not found, return item not found response
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Check if the user has already reacted to the blog
                var existingReaction = await dbContext.BlogReactions.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.BlogReactionModel.UserId) && comment.BlogId == request.BlogReactionModel.BlogId, cancellationToken);

                if (existingReaction == null)
                {
                    // If there is no existing reaction, create a new one
                    var newReaction = request.BlogReactionModel.Adapt(new BlogReaction());
                    dbContext.BlogReactions.Add(newReaction);

                    // Add notification based on the reaction type
                    if (newReaction.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is downvoted by " + user.Name
                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (newReaction.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is upvoted by " + user.Name
                        };
                        dbContext.Notifications.Add(notification);
                    }

                    // Save changes to the database
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Return item created response
                    return GeneralDbResponses.ItemCreated("Reaction");
                }
                else
                {
                    // If there is an existing reaction, update it
                    dbContext.Entry(existingReaction).State = EntityState.Detached;
                    var updatedReaction = request.BlogReactionModel.Adapt(new BlogReaction());
                    updatedReaction.UpdatedAt = DateTime.Now;
                    dbContext.BlogReactions.Update(updatedReaction);

                    // Add notification based on the updated reaction type
                    if (updatedReaction.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is updated to downvote by " + user.Name
                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (updatedReaction.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is updated to upvote by " + user.Name
                        };
                        dbContext.Notifications.Add(notification);
                    }

                    // Save changes to the database
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Return item updated response
                    return GeneralDbResponses.ItemUpdate("Comment");
                }
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs
                return new ServiceResponse(true, ex.Message);
            }
        }

    }
}
