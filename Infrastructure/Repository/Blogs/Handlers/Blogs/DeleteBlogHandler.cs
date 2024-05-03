using Application.DTO.Response;
using Application.Service.Blogs.Commands.Blogs;
using Infrastructure.DataAccess.Blogs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    public class DeleteBlogHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<DeleteBlogCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var data = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.Id), cancellationToken: cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("BLog");
                }
                dbContext.Blogs.Remove(data);
                await dbContext.SaveChangesAsync(cancellationToken);

                var blogImages = await dbContext.BlogImages.FirstAsync(_=>_.BlogId.Equals(request.Id),cancellationToken:cancellationToken);
                // Assuming dbContext is your instance of DbContext and blogImages is your collection
                dbContext.BlogImages.RemoveRange(blogImages);
                await dbContext.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemDelete(data.Title);
            }catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
