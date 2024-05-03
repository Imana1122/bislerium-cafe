using Application.DTO.Response;
using Application.Service.Blogs.Commands.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess.Blogs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Products.Handlers.Blogs
{
    public class CreateBlogHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<CreateBlogCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
               

                var data = request.BlogModel.Adapt(new Blog());
                dbContext.Blogs.Add(data);
                await dbContext.SaveChangesAsync(cancellationToken);

                foreach (var blog in request.BlogImages)
                { 
                    var imageData = blog.Adapt(new BlogImage());
                    dbContext.BlogImages.Add(imageData);
                    await dbContext.SaveChangesAsync(cancellationToken);

                }
                return GeneralDbResponses.ItemCreated(request.BlogModel.Title);
            }
            catch(Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
           

        }
    }
}
