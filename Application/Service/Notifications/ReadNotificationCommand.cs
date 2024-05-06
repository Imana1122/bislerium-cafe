using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Notifications
{
    public record ReadNotificationCommand(Guid Id) :IRequest<ServiceResponse>;
   
}
