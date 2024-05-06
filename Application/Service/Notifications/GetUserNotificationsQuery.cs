using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Notifications
{
    public record GetUserNotificationsQuery(Guid UserId): IRequest<IEnumerable<Notification>>;
   
}
