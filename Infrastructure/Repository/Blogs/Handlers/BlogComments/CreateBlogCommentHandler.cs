using Application.DTO.Response;
using Application.Service.Blogs.Commands.BlogComments;
using Application.Service.Blogs.Commands.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess.Blogs;
using Infrastructure.Repository.Products;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    public class CreateBlogCommentHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<CreateBlogCommentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateBlogCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();


                var data = request.BlogCommentModel.Adapt(new BlogComment());
                dbContext.BlogComments.Add(data);
                await dbContext.SaveChangesAsync(cancellationToken);


                return GeneralDbResponses.ItemCreated("Comment");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }


        }
    }
}
