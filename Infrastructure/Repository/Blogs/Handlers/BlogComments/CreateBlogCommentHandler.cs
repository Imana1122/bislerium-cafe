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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    public class CreateBlogCommentHandler(IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<CreateBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();

                var user = await userManager.FindByIdAsync(request.BlogCommentModel.UserId.ToString());
                if (user == null)
                {
                    return GeneralDbResponses.ItemNotFound("User");
                }
                var data = request.BlogCommentModel.Adapt(new BlogComment());
                dbContext.BlogComments.Add(data);
                await dbContext.SaveChangesAsync(cancellationToken);


                return GeneralDbResponses.ItemCreated("Comment");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }


        }
    }
}
