using Application.Extensions.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response.Bloggers
{
    public class GetBloggerResponseDTO 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Policy { get; set; }
        public int BlogsCount { get; set; }
        public int PopularityCount { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }



    }
}
