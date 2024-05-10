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
    //Handler to get user history 
    public class GetUserHistoryHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetUserHistoryQuery, IEnumerable<History>>
    {
        public async Task<IEnumerable<History>> Handle(GetUserHistoryQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the database context using the factory
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve user history records from the database based on the provided user ID
            var data = await dbContext.Histories
                .Where(_ => _.UserId.Equals(request.UserId)) // Filter by the provided user ID
                .AsNoTracking() // Ensure entities are read-only to improve performance
                .ToListAsync(cancellationToken); // Materialize the query into a list asynchronously

            // Return the list of user history records
            return data;
        }



    }
}
