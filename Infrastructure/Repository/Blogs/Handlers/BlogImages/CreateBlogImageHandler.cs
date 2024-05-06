using Application.DTO.Request.Blogs;
using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.BlogImages;
using Domain.Entities;
using Infrastructure.DataAccess;
using Infrastructure.Repository.Products;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogImages
{
    public class CreateBlogImageHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<CreateBlogImageCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateBlogImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();

                var blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.imageModel.BlogId), cancellationToken);
                if (blog == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }
                var data = request.imageModel.Adapt(new BlogImage());
                dbContext.BlogImages.Add(data);
                await dbContext.SaveChangesAsync(cancellationToken);


                return GeneralDbResponses.ItemCreated("Image");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }


        }
    }
}
