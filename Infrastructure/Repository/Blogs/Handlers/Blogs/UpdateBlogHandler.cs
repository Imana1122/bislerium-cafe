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

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    public class UpdateBlogHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) :IRequestHandler<UpdateBlogCommand,ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var category = await dbContext.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(request.BlogModel.Id),cancellationToken:cancellationToken);
                if (category == null)
                {
                    return GeneralDbResponses.ItemNotFound(request.BlogModel.Title);
                }
                dbContext.Entry(category).State = EntityState.Detached;
                var adaptData = request.BlogModel.Adapt(new Blog());
                dbContext.Blogs.Update(adaptData);
                await dbContext.SaveChangesAsync(cancellationToken);
                return GeneralDbResponses.ItemUpdate(request.BlogModel.Title);
            }catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }

    }
}
