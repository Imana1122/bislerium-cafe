using Application.DTO.Request.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTO.Response.Blogs
{
    public class GetBlogsResponseDTO : BlogBaseDTO
    {
        public Guid Id { get; set; }
       
     
        public GetBlogUpvoteCountResponseDTO UpvoteCount {get;set;}
        public GetBlogDownvoteCountResponseDTO DownvoteCount { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
