using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.BlogImages;
using Infrastructure.DataAccess.Blogs;
using Infrastructure.Repository.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogImages
{
    public class DeleteBlogImageHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<DeleteBlogImageCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var data = await dbContext.BlogImages.FirstOrDefaultAsync(_ => _.Id.Equals(request.Id), cancellationToken: cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("BLog Image");
                }
                dbContext.BlogImages.Remove(data);
                await dbContext.SaveChangesAsync(cancellationToken);



                return GeneralDbResponses.ItemDelete("Image");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
