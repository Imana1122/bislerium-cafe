using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.BlogCommentReactions;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogCommentReactions
{
    //Handler to update blog comment reaction
    public class UpdateBlogCommentReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateBlogCommentReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogCommentReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Find the user by ID using the user manager
                var user = await userManager.FindByIdAsync(request.model.UserId.ToString());

                // If the user is not found, return item not found response
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                // Find the commented user by ID using the user manager
                var commentUser = await userManager.FindByIdAsync(request.model.CommentUserId.ToString());

                // If the commented user is not found, return item not found response
                if (commentUser == null)
                {
                    return GeneralDbResponses.ItemNotFound("Commented User");
                }

                // Retrieve the blog from the database by ID
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.model.BlogId), cancellationToken);

                // If the blog with the specified ID is not found, return item not found response
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Retrieve the comment associated with the blog from the database by BlogId and CommentUserId
                var comment = await dbContext.BlogComments.FirstOrDefaultAsync(_ => _.BlogId.Equals(request.model.BlogId) && _.UserId.Equals(request.model.CommentUserId), cancellationToken);

                // If the comment associated with the blog is not found, return item not found response
                if (comment == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog Comment");
                }

                // Retrieve the blog comment reaction from the database by UserId, BlogId, and CommentUserId
                var data = await dbContext.BlogCommentReactions.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.model.UserId) && comment.BlogId == request.model.BlogId && comment.CommentUserId.Equals(request.model.CommentUserId), cancellationToken);

                // If the blog comment reaction is not found, create a new one; otherwise, update it
                if (data == null)
                {
                    var newdata = request.model.Adapt(new BlogCommentReaction());
                    newdata.UpdatedAt = DateTime.Now;
                    dbContext.BlogCommentReactions.Add(newdata);

                    // Add notification based on the reaction type
                    if (newdata.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = $"Your comment on {blog.Title} is downvoted by {user.Name}"
                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (newdata.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = $"Your comment on {blog.Title} is upvoted by {user.Name}"
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
                    // If there is an existing blog comment reaction, update it
                    dbContext.Entry(data).State = EntityState.Detached;
                    var adaptData = request.model.Adapt(new BlogCommentReaction());
                    adaptData.CreatedAt = DateTime.Now;
                    dbContext.BlogCommentReactions.Update(adaptData);

                    // Add notification based on the updated reaction type
                    if (adaptData.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = $"Your comment on {blog.Title} is updated to downvote by {user.Name}"
                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (adaptData.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = $"Your comment on {blog.Title} is updated to upvote by {user.Name}"
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
