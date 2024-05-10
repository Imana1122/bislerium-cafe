using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.DTO.Response.Bloggers;
using Application.DTO.Response.Identity;
using Application.Extensions.Identity;
using Application.Service.Bloggers;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Blogs.Handlers.Bloggers
{
    //Handler to get bloggers data
    public class GetBloggersHandler : IRequestHandler<GetBloggersQuery, IEnumerable<GetBloggerResponseDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetBloggersHandler(UserManager<ApplicationUser> userManager)
        {
            // Constructor injection: UserManager is injected into the handler
            _userManager = userManager;
        }

        public async Task<IEnumerable<GetBloggerResponseDTO>> Handle(GetBloggersQuery request, CancellationToken cancellationToken)
        {
            // Retrieve users with role "Blogger" using UserManager
            var bloggers = await _userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Blogger"));

            // Map bloggers to DTOs using Automapper
            var bloggerDTOs = bloggers.Select(blog => blog.Adapt<GetBloggerResponseDTO>());

            // Return the DTOs representing the bloggers
            return bloggerDTOs;
        }
    }

}
