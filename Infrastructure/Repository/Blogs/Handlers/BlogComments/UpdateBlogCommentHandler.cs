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
    public class UpdateBlogCommentHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var user = await userManager.FindByIdAsync(request.BlogCommentModel.UserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogCommentModel.BlogId) , cancellationToken);
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                var data = await dbContext.BlogComments.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.BlogCommentModel.UserId) && comment.BlogId == request.BlogCommentModel.BlogId, cancellationToken);
                if (data == null)
                {
                    var comment = request.BlogCommentModel.Adapt(new BlogComment());

                    dbContext.BlogComments.Add(comment);
                    var history = new UserHistory
                    {
                        UserId = comment.UserId,
                        Content = " You commented on " + blog.Title

                    };


                    dbContext.Histories.Add(history);

                    var newnotification = new Notification
                    {
                        UserId = blog.UserId,
                        Content = user.Name + " commented on " + blog.Title

                    };
                    dbContext.Notifications.Add(newnotification);

                    await dbContext.SaveChangesAsync(cancellationToken);

                    return GeneralDbResponses.ItemCreated("Comment");

                }
                else
                {



                    dbContext.Entry(data).State = EntityState.Detached;
                    var adaptData = request.BlogCommentModel.Adapt(new BlogComment());
                    dbContext.BlogComments.Update(adaptData);

                    var updatehistory = new UserHistory
                    {
                        UserId = adaptData.UserId,
                        Content = " You updated your comment on " + blog.Title

                    };



                    dbContext.Histories.Add(updatehistory);

                    var notification = new Notification
                    {
                        Read = false,
                        UserId = blog.UserId,
                        Content = user.Name + " updated comment on " + blog.Title

                    };
                    dbContext.Notifications.Add(notification);

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
