using Application.DTO.Response.Blogs;
using NetcodeHub.Packages.Extensions.Attributes.RequiredGuid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Request.Blogs
{
    public class AddBlogReactionRequestDTO
    {
        [Required]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]

        [NetcodeHubRequiredGuid(ErrorMessage = "Blog is required")]
        public Guid BlogId { get; set; }
        [Required]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]

        [NetcodeHubRequiredGuid(ErrorMessage = "User is required")]
        public Guid UserId { get; set; }

        [Required]
        public string Reaction { get; set; }
    }
}
