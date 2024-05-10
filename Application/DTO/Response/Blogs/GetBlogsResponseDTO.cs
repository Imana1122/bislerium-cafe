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
        public string BloggerName { get; set; }
        public int UpvoteCount {get;set;}
        public int DownvoteCount { get; set; }
        public int CommentsCount { get; set; }
        public int PopularityCount { get; set; }


        public bool UpvotedStatus { get; set; }

        public bool DownvotedStatus { get; set; }

        public DateTime CreatedAt { get; set; } =DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
