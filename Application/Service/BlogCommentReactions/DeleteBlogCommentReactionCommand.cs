using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BlogCommentReactions
{
    //for deleting blog comment reaction
  
    public record DeleteBlogCommentReactionCommand(Guid UserId, Guid BlogId,Guid CommentUserId) : IRequest<ServiceResponse>;

  
}
