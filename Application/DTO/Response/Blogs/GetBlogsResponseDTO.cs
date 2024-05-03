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
        [JsonIgnore]
        public virtual ICollection<GetBlogImageResponseDTO> Images { get; set; } = null;

        public GetBlogUpvoteCountResponseDTO UpvoteCount {get;set;}
        public GetBlogDownvoteCountResponseDTO DownvoteCount { get; set; }
        public GetBlogCommentsCountResponseDTO CommentsCount { get; set; }


        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
