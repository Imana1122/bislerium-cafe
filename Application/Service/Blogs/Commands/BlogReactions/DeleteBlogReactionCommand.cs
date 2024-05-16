using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Commands.BlogReactions
{
    //for deleting blog reaction
    public record DeleteBlogReactionCommand(Guid UserId, Guid BlogId) : IRequest<ServiceResponse>;
   
}
