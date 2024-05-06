using Application.DTO.Response.Bloggers;
using Application.Extensions.Identity;
using Application.Service.Admins;
using Application.Service.Bloggers;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.Admin
{
    public class GetAdminsHandler : IRequestHandler<GetAdminsQuery, IEnumerable<ApplicationUser>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAdminsHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
        {
            // Retrieve users with role "Admin"
            var admins = await _userManager.Users.Where(_ => _.Policy == "AdminPolicy" || _.Policy=="ManagerPolicy").ToListAsync();
           


            // Adapt user entities to DTOs if needed
            var adminAndManagerDTOs = admins.Select(user => user.Adapt<ApplicationUser>());

            return adminAndManagerDTOs;
        }
    }
}
