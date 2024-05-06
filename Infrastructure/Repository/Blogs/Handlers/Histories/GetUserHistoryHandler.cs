using Application.DTO.Response.Blogs;
using Application.Service.Blogs.Queries.Blogs;
using Application.Service.Histories;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.Histories
{
    public class GetUserHistoryHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetUserHistoryQuery, IEnumerable<UserHistory>>
    {
        public async Task<IEnumerable<UserHistory>> Handle(GetUserHistoryQuery request, CancellationToken cancellationToken)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var data = await dbContext.Histories
                .Where(_ => _.UserId.Equals(request.UserId))                
                .AsNoTracking()
                .ToListAsync(cancellationToken);

           

            return data;
        }
        


    }
}
