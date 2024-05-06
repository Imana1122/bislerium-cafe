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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    public class UpdateBlogHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) :IRequestHandler<UpdateBlogCommand,ServiceResponse>
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

                var history = new UserHistory
                {
                    UserId = category.UserId,
                    Content = category.Title + " is updated."

                };

                dbContext.Histories.Add(history);

                await dbContext.SaveChangesAsync(cancellationToken);
                return GeneralDbResponses.ItemUpdate(request.BlogModel.Title);
            }catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }

    }
}
