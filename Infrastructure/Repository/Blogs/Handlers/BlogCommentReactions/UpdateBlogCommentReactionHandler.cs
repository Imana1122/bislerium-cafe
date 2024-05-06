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
    public class UpdateBlogCommentReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateBlogCommentReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogCommentReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var user = await userManager.FindByIdAsync(request.model.UserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }
                var commentUser = await userManager.FindByIdAsync(request.model.CommentUserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("Commented User");
                }

                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.model.BlogId), cancellationToken);
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                var comment = await dbContext.BlogComments.FirstOrDefaultAsync(_ => _.BlogId.Equals(request.model.BlogId) && _.UserId.Equals(request.model.CommentUserId), cancellationToken);
                if (comment == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                var data = await dbContext.BlogCommentReactions.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.model.UserId) && comment.BlogId == request.model.BlogId && comment.CommentUserId.Equals(request.model.CommentUserId), cancellationToken);
                if (data == null)
                {
                    var newdata = request.model.Adapt(new BlogCommentReaction());
                    dbContext.BlogCommentReactions.Add(newdata);
                    if (newdata.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = "Your comment on "+blog.Title + " is downvoted by " + user.Name

                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (newdata.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = "Your comment on " + blog.Title + " is upvoted by " + user.Name

                        };
                        dbContext.Notifications.Add(notification);

                    }

                    await dbContext.SaveChangesAsync(cancellationToken);


                    return GeneralDbResponses.ItemCreated("Reaction");
                }
                else
                {


                    dbContext.Entry(data).State = EntityState.Detached;
                    var adaptData = request.model.Adapt(new BlogCommentReaction());
                    dbContext.BlogCommentReactions.Update(adaptData);
                    if (adaptData.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = "Your comment on " + blog.Title + " is updated to downvote by " + user.Name

                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (adaptData.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = comment.UserId,
                            Content = "Your comment on " + blog.Title + " is updated to upvote by " + user.Name

                        };
                        dbContext.Notifications.Add(notification);

                    }
                    await dbContext.SaveChangesAsync(cancellationToken);
                    return GeneralDbResponses.ItemUpdate("Comment");
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
