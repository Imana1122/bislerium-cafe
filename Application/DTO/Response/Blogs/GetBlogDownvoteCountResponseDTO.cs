using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response.Blogs
{
    //respose model for fetching downvote count
    public record GetBlogDownvoteCountResponseDTO(int DownvoteCount);
    
}
