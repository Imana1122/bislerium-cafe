using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.Blogs;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    //Handle update or create blog comment
    public class UpdateBlogCommentHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create a new instance of the database context using the factory
                using var dbContext = contextFactory.CreateDbContext();

                // Find the user associated with the provided user ID
                var user = await userManager.FindByIdAsync(request.BlogCommentModel.UserId.ToString());
                if (user == null)
                {
                    // If user not found, return a response indicating that the user was not found
                    return GeneralDbResponses.ItemNotFound("User");
                }

                // Find the blog associated with the provided blog ID
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogCommentModel.BlogId), cancellationToken);
                if (blog == null)
                {
                    // If blog not found, return a response indicating that the blog was not found
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Find if the blog comment already exists for the given user and blog
                var existingComment = await dbContext.BlogComments.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.BlogCommentModel.UserId) && comment.BlogId == request.BlogCommentModel.BlogId, cancellationToken);
                if (existingComment == null)
                {
                    // If no existing comment found, create a new comment
                    var newComment = request.BlogCommentModel.Adapt(new BlogComment());
                    dbContext.BlogComments.Add(newComment);

                    // Create a history entry indicating that a new comment was added
                    var history = new History
                    {
                        UserId = newComment.UserId,
                        Content = " You commented on " + blog.Title
                    };
                    dbContext.Histories.Add(history);

                    // Create a notification for the blog owner indicating that a new comment was added
                    var newNotification = new Notification
                    {
                        UserId = blog.UserId,
                        Content = user.Name + " commented on " + blog.Title
                    };
                    dbContext.Notifications.Add(newNotification);

                    // Save changes to the database
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Return a response indicating that the comment was created
                    return GeneralDbResponses.ItemCreated("Comment");
                }
                else
                {
                    // If an existing comment is found, update the existing comment
                    dbContext.Entry(existingComment).State = EntityState.Detached;
                    var updatedComment = request.BlogCommentModel.Adapt(new BlogComment());
                    updatedComment.UpdatedAt = DateTime.Now;
                    dbContext.BlogComments.Update(updatedComment);

                    // Create a history entry indicating that the comment was updated
                    var updateHistory = new History
                    {
                        UserId = updatedComment.UserId,
                        Content = " You updated your comment on " + blog.Title
                    };
                    dbContext.Histories.Add(updateHistory);

                    // Create a notification for the blog owner indicating that the comment was updated
                    var notification = new Notification
                    {
                        Read = false,
                        UserId = blog.UserId,
                        Content = user.Name + " updated comment on " + blog.Title
                    };
                    dbContext.Notifications.Add(notification);

                    // Save changes to the database
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Return a response indicating that the comment was updated
                    return GeneralDbResponses.ItemUpdate("Comment");
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs during the execution, return a response with an error message
                return new ServiceResponse(true, ex.Message);
            }
        }

    }
}
