using Application.Service.Histories;
using Application.Service.Notifications;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.Notifications
{
    //Handler to get user notifications of a specific user 
    public class GetUserNotificationsHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetUserNotificationsQuery, IEnumerable<Notification>>
    {
        public async Task<IEnumerable<Notification>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the database context using the factory
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve notifications for the specified user ID from the database
            var data = await dbContext.Notifications
                .Where(_ => _.UserId.Equals(request.UserId)) // Filter by user ID
                .AsNoTracking() // Ensure entities are read-only to improve performance
                .ToListAsync(cancellationToken); // Materialize the query into a list asynchronously

            // Return the list of user notifications
            return data;
        }




    }
}
