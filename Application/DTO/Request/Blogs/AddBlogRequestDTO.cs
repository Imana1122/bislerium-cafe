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
    //request model for adding blog
    public class AddBlogRequestDTO:BlogBaseDTO
    {
        [Required]
        public ICollection<AddBlogImageRequestDTO> Images;

    }
}
