using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.Blogs;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    //Handler to delete blog comment
    public class DeleteBlogCommentHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Find the user by ID using the user manager
                var user = await userManager.FindByIdAsync(request.UserId.ToString());

                // If the user is not found, return item not found response
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                // Retrieve the blog from the database by ID
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogId), cancellationToken);

                // If the blog with the specified ID is not found, return item not found response
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Retrieve the comment from the database by UserId and BlogId
                var data = await dbContext.BlogComments
                    .Include(comment => comment.Reactions) // Include related reactions
                    .FirstOrDefaultAsync(comment => comment.UserId.ToString().ToLower().Equals(request.UserId.ToString().ToLower()) && comment.BlogId.ToString().ToLower() == request.BlogId.ToString().ToLower(), cancellationToken);

                // If the comment is not found, return item not found response
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog Comment");
                }

                // If there are reactions associated with the comment, remove them
                if (data.Reactions != null)
                {
                    foreach (var item in data.Reactions)
                    {
                        dbContext.BlogCommentReactions.Remove(item);
                    }
                }

                // Save changes after removing reactions
                await dbContext.SaveChangesAsync(cancellationToken);

                // Remove the comment itself
                dbContext.BlogComments.Remove(data);

                // Save changes after removing the comment
                await dbContext.SaveChangesAsync(cancellationToken);

                // Add history entry for the user who deleted the comment
                var history = new History
                {
                    UserId = data.UserId,
                    Content = " You deleted your comment on " + blog.Title
                };
                dbContext.Histories.Add(history);

                // Create a notification for the owner of the blog
                var notification = new Notification
                {
                    Read = false,
                    UserId = blog.UserId,
                    Content = user.Name + " deleted comment on " + blog.Title
                };
                dbContext.Notifications.Add(notification);

                // Save changes after adding history and notification
                await dbContext.SaveChangesAsync(cancellationToken);

                // Return item delete response
                return GeneralDbResponses.ItemDelete("Comment");
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs
                return new ServiceResponse(true, ex.Message);
            }
        }

    }
}
