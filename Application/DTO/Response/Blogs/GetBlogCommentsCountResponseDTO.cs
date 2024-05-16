using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response.Blogs
{
    //response model for fetching blog comment count
    public record GetBlogCommentsCountResponseDTO(int CommentsCount);
   
}
