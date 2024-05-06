using Application.DTO.Response;
using Application.Service.Blogs.Commands.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
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
    public class CreateBlogHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<CreateBlogCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
               

                var data = request.BlogModel.Adapt(new Blog());
                dbContext.Blogs.Add(data);
                await dbContext.SaveChangesAsync(cancellationToken);



                foreach (var blog in request.BlogModel.Images)
                {
                    var imageData = blog.Adapt(new BlogImage());
                    imageData.BlogId = data.Id;
                    dbContext.BlogImages.Add(imageData);
                    await dbContext.SaveChangesAsync(cancellationToken);

                }

                var history = new UserHistory
                {
                    UserId=request.BlogModel.UserId,
                    Content=request.BlogModel.Title+" is created."

                };
                dbContext.Histories.Add(history);

                await dbContext.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemCreated(request.BlogModel.Title);
            }
            catch(Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
           

        }
    }
}
