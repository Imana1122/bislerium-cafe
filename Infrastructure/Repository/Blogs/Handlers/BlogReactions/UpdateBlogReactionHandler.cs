using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.BlogReactions;
using Domain.Entities;
using Infrastructure.DataAccess.Blogs;
using Infrastructure.Repository.Products;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogReactions
{
    public class UpdateBlogReactionHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<UpdateBlogReactionCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var data = await dbContext.BlogReactions.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.BlogReactionModel.UserId) && comment.BlogId == request.BlogReactionModel.BlogId, cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Comment");
                }
                dbContext.Entry(data).State = EntityState.Detached;
                var adaptData = request.BlogReactionModel.Adapt(new BlogReaction());
                dbContext.BlogReactions.Update(adaptData);
                await dbContext.SaveChangesAsync(cancellationToken);
                return GeneralDbResponses.ItemUpdate("Comment");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
