using Application.DTO.Request.Blogs;
using Application.Extensions.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTO.Response.Blogs
{
    public class GetBlogCommentResponseDTO:UpdateBlogCommentRequestDTO
    {

        [Required]
        public ApplicationUser User;
        public int UpvoteCount { get; set; }

        public int DownvoteCount { get; set; }


        public bool UpvotedStatus { get; set; }

        public bool DownvotedStatus { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
