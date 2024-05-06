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
    public class GetUnreadUserNotificationsCountHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetUnreadUserNotificationCountQuery, int>
    {
        public async Task<int> Handle(GetUnreadUserNotificationCountQuery request, CancellationToken cancellationToken)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var data = await dbContext.Notifications
                .Where(_ => _.UserId.Equals(request.UserId) && _.Read != true)
                .AsNoTracking()
                .ToListAsync(cancellationToken);



            return data.Count();
        }



    }
}
