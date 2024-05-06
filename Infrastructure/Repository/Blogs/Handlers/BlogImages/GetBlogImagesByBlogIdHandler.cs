using Application.DTO.Response.Blogs;
using Application.Extensions.Identity;
using Application.Service.Blogs.Queries.BlogComments;
using Application.Service.Blogs.Queries.BlogImages;
using Domain.Entities;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogImages
{
    public class GetBlogImagesByBlogIdHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetBlogImagesByBlogIdQuery, IEnumerable<BlogImage>>
    {
        public async Task<IEnumerable<BlogImage>> Handle(GetBlogImagesByBlogIdQuery request, CancellationToken cancellationToken)
        {

            using var dbContext = contextFactory.CreateDbContext();

            var images = await dbContext.BlogImages
                .Where(comment => comment.BlogId.Equals(request.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var responseDTOs = new List<BlogImage>();

            foreach (var image in images)
            {

                // Map comment entity to DTO
                var dataDTO = image.Adapt<BlogImage>();
                responseDTOs.Add(dataDTO);
            }

            return responseDTOs;
        }

     
    }
}
