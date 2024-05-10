using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.BlogCommentReactions;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogCommentReactions
{
    //Handler to delete blog comment reaction
    public class DeleteBlogCommentReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteBlogCommentReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogCommentReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Retrieve the blog comment from the database by BlogId and UserId
                var comment = await dbContext.BlogComments.FirstOrDefaultAsync(_ => _.BlogId.Equals(request.BlogId) && _.UserId.Equals(request.UserId), cancellationToken);

                // If the blog comment with the specified BlogId and UserId is not found, return item not found response
                if (comment == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog Comment");
                }

                // Retrieve the blog associated with the comment from the database by BlogId
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(comment.BlogId), cancellationToken);

                // If the blog with the specified BlogId is not found, return item not found response
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Find the user who performed the action by UserId
                var user = await userManager.FindByIdAsync(request.UserId.ToString());

                // If the user is not found, return item not found response
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                // Find the user associated with the comment by CommentUserId
                var commentUser = await userManager.FindByIdAsync(request.CommentUserId.ToString());

                // If the user associated with the comment is not found, return item not found response
                if (commentUser == null)
                {
                    return GeneralDbResponses.ItemNotFound("Comment User");
                }

                // Retrieve the blog comment reaction from the database by UserId, BlogId, and CommentUserId
                var data = await dbContext.BlogCommentReactions.FirstOrDefaultAsync(_ => _.UserId.Equals(request.UserId) && _.BlogId == request.BlogId && _.CommentUserId.Equals(request.CommentUserId), cancellationToken);

                // If the blog comment reaction is not found, return item not found response
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog comment reaction");
                }

                // Remove the blog comment reaction from the database
                dbContext.BlogCommentReactions.Remove(data);

                // Create a notification for the owner of the comment
                var notification = new Notification
                {
                    Read = false,
                    UserId = comment.UserId,
                    Content = $"{user.Name} unreacted on your comment on {blog.Title}"
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

