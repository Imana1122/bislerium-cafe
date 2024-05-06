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
    public class DeleteBlogCommentReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteBlogCommentReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogCommentReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                
                var comment = await dbContext.BlogComments.FirstOrDefaultAsync(_ => _.BlogId.Equals(request.BlogId) && _.UserId.Equals(request.UserId), cancellationToken);
                if (comment == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog Comment");
                }
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(comment.BlogId), cancellationToken);
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }
                var user = await userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                var commentuser = await userManager.FindByIdAsync(request.CommentUserId.ToString());
                if (commentuser == null)
                {
                    return GeneralDbResponses.ItemNotFound("Comment User");
                }

                var data = await dbContext.BlogCommentReactions
                    .FirstOrDefaultAsync(_ => _.UserId.Equals(request.UserId) && _.BlogId == request.BlogId && _.CommentUserId.Equals(request.CommentUserId), cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog comment reaction");
                }
                dbContext.BlogCommentReactions.Remove(data);

                var notification = new Notification
                {
                    Read = false,
                    UserId = comment.UserId,
                    Content =  user.Name+" unreacted on your comment on "+blog.Title

                };
                dbContext.Notifications.Add(notification);


                await dbContext.SaveChangesAsync(cancellationToken);



                return GeneralDbResponses.ItemDelete("Reaction");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
