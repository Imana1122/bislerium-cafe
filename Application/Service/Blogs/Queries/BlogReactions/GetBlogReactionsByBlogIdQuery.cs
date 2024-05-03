using Application.DTO.Response.Blogs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Queries.BlogReactions
{
    public record GetBlogReactionsByBlogIdQuery(Guid Id) : IRequest<IEnumerable<GetBlogReactionResponseDTO>>;
  
}
