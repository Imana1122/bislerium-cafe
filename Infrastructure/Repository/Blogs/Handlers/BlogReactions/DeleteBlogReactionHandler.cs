using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.Blogs.Commands.BlogComments;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogReactions
{
    public class DeleteBlogReactionHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteBlogReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogId), cancellationToken);
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }
                var user = await userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                var data = await dbContext.BlogReactions
                    .FirstOrDefaultAsync(_ => _.UserId.Equals(request.UserId) && _.BlogId == request.BlogId, cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog reaction");
                }
                dbContext.BlogReactions.Remove(data);
                
                    var notification = new Notification
                    {
                        Read = false,
                        UserId = blog.UserId,
                        Content = blog.Title + " is unreacted by " + user.Name

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
