using Application.DTO.Response.Blogs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Queries.BlogComments
{
    //for getting all blog comments by blog id
    public record GetAllBlogCommentsByBlogIdQuery(Guid BlogId, Guid? UserId) :IRequest<IEnumerable<GetBlogCommentResponseDTO>>;
   
}
