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
    public class UpdateBlogReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateBlogReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var user = await userManager.FindByIdAsync(request.BlogReactionModel.UserId.ToString());
                if(user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogReactionModel.BlogId), cancellationToken);
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                var data = await dbContext.BlogReactions.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.BlogReactionModel.UserId) && comment.BlogId == request.BlogReactionModel.BlogId, cancellationToken);
                if (data == null)
                {
                    var newdata = request.BlogReactionModel.Adapt(new BlogReaction());
                    dbContext.BlogReactions.Add(newdata);
                    if (newdata.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is downvoted by " + user.Name

                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (newdata.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is upvoted by " + user.Name

                        };
                        dbContext.Notifications.Add(notification);

                    }

                    await dbContext.SaveChangesAsync(cancellationToken);


                    return GeneralDbResponses.ItemCreated("Reaction");
                }
                else
                {


                    dbContext.Entry(data).State = EntityState.Detached;
                    var adaptData = request.BlogReactionModel.Adapt(new BlogReaction());
                    dbContext.BlogReactions.Update(adaptData);
                    if (adaptData.Reaction == "Downvote")
                    {
                        var notification = new Notification
                        {
                            Read = false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is updated to downvote by " + user.Name

                        };
                        dbContext.Notifications.Add(notification);
                    }
                    else if (adaptData.Reaction == "Upvote")
                    {
                        var notification = new Notification
                        {
                            Read=false,
                            UserId = blog.UserId,
                            Content = blog.Title + " is updated to downvote by " + user.Name

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
