using Application.DTO.Response.Dashboard;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Dashboard
{
    public record GetDashboardDataQuery(int? month):IRequest<DashboardDataDTO>;
   
}
