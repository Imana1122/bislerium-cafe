using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Histories
{
    public record GetUserHistoryQuery(Guid UserId): IRequest<IEnumerable<UserHistory>>;
   
}
