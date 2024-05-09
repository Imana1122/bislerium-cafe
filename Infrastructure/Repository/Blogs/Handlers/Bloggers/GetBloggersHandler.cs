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
    public class GetBloggersHandler : IRequestHandler<GetBloggersQuery, IEnumerable<GetBloggerResponseDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetBloggersHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<GetBloggerResponseDTO>> Handle(GetBloggersQuery request, CancellationToken cancellationToken)
        {
            // Retrieve users with role "User"
            var bloggers = await _userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Blogger"));



            var bloggerDTOs = bloggers.Select(blog => blog.Adapt<GetBloggerResponseDTO>());

            return bloggerDTOs;
        }
    }
}
