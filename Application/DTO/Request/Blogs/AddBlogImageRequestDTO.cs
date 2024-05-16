using NetcodeHub.Packages.Extensions.Attributes.RequiredGuid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Request.Blogs
{
    //request model for adding new image while creating blog and it's images
    public class AddBlogImageRequestDTO
    {
       

        [Required]
        public string Image { get; set; }
       
    }
}
