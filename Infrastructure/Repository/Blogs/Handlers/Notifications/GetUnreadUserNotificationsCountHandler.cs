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
    //Handler to get unread notifications count of a user
    public class GetUnreadUserNotificationsCountHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetUnreadUserNotificationCountQuery, int>
    {
        public async Task<int> Handle(GetUnreadUserNotificationCountQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the database context using the factory
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve unread notifications for the specified user ID from the database
            var data = await dbContext.Notifications
                .Where(_ => _.UserId.Equals(request.UserId) && _.Read != true) // Filter by user ID and unread status
                .AsNoTracking() // Ensure entities are read-only to improve performance
                .ToListAsync(cancellationToken); // Materialize the query into a list asynchronously

            // Return the count of unread notifications
            return data.Count();
        }




    }
}
