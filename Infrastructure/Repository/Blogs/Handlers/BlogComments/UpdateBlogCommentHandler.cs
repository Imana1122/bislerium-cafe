using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.Blogs;
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

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    public class UpdateBlogCommentHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<UpdateBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var data = await dbContext.BlogComments.FirstOrDefaultAsync(comment => comment.UserId.Equals(request.BlogCommentModel.UserId) && comment.BlogId == request.BlogCommentModel.BlogId, cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Comment");
                }
                dbContext.Entry(data).State = EntityState.Detached;
                var adaptData = request.BlogCommentModel.Adapt(new BlogComment());
                dbContext.BlogComments.Update(adaptData);
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
