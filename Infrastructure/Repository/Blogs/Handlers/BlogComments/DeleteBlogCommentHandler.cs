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
    public class DeleteBlogCommentHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var user = await userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogId), cancellationToken);
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }
                var data = await dbContext.BlogComments
    .Include(comment => comment.Reactions) // Include related reactions
    .FirstOrDefaultAsync(comment => comment.UserId.ToString().ToLower().Equals(request.UserId.ToString().ToLower()) && comment.BlogId.ToString().ToLower() == request.BlogId.ToString().ToLower(), cancellationToken);

                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog Comment");
                }

                if (data.Reactions != null)
                {
                    foreach (var item in data.Reactions)
                    {
                        dbContext.BlogCommentReactions.Remove(item);
                    }
                }

                // Save changes after removing reactions
                await dbContext.SaveChangesAsync(cancellationToken);

                // Then remove the comment itself
                dbContext.BlogComments.Remove(data);

                // Save changes after removing the comment
                await dbContext.SaveChangesAsync(cancellationToken);


                dbContext.BlogComments.Remove(data);
                
                var history = new UserHistory
                {
                    UserId = data.UserId,
                    Content = " You deleted your comment on " + blog.Title

                };

                dbContext.Histories.Add(history);

                var notification = new Notification
                {
                    Read = false,
                    UserId = blog.UserId,
                    Content = user.Name + " deleted comment on " + blog.Title

                };
                dbContext.Notifications.Add(notification);

                await dbContext.SaveChangesAsync(cancellationToken);

               

                return GeneralDbResponses.ItemDelete("Comment");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
