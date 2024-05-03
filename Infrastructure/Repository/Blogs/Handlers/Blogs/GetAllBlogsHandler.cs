using Application.DTO.Response.Blogs;
using Application.Service.Blogs.Queries.Blogs;
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
    public class GetAllBlogsHandler(DataAccess.Blogs.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetAllBlogsQuery, IEnumerable<GetBlogsResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogsResponseDTO>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var data = await dbContext.Blogs.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
            return data.Adapt<List<GetBlogsResponseDTO>>();
        }
    }
}
