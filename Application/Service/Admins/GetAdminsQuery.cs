using Application.Extensions.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Admins
{
    public record GetAdminsQuery : IRequest<IEnumerable<ApplicationUser>>;
   
}
