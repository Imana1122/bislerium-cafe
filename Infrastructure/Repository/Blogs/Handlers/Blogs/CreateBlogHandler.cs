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
                var Blog = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Title.ToLower().Equals(request.BlogModel.Title.ToLower()),cancellationToken:cancellationToken);
                if (Blog == null)
                {
                    return GeneralDbResponses.ItemAlreadyExist(request.BlogModel.Title);
                }

                var data = request.BlogModel.Adapt(new Blog());
                dbContext.Blogs.Add(data);
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
