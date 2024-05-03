using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.Blogs;
using Infrastructure.DataAccess.Blogs;
using Infrastructure.Repository.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    public class DeleteBlogCommentHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<DeleteBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var data = await dbContext.BlogComments
                    .FirstOrDefaultAsync(comment => comment.UserId.Equals(request.UserId) && comment.BlogId == request.BlogId, cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("BLog Comment");
                }
                dbContext.BlogComments.Remove(data);
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
