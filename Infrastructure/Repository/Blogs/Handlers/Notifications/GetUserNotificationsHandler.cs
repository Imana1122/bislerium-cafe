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
    public class GetUserNotificationsHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetUserNotificationsQuery, IEnumerable<Notification>>
    {
        public async Task<IEnumerable<Notification>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var data = await dbContext.Notifications
                .Where(_ => _.UserId.Equals(request.UserId))
                .AsNoTracking()
                .ToListAsync(cancellationToken);



            return data;
        }



    }
}
