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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogReactions
{
    public class CreateBlogReactionHandler(IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<CreateBlogReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateBlogReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var user = await userManager.FindByIdAsync(request.BlogReactionModel.UserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }

                var data = request.BlogReactionModel.Adapt(new BlogReaction());
                dbContext.BlogReactions.Add(data);
                await dbContext.SaveChangesAsync(cancellationToken);


                return GeneralDbResponses.ItemCreated("Reaction");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }


        }
    }
}
