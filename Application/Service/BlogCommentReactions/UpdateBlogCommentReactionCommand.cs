using Application.DTO.Request.BlogCommentReactions;
using Application.DTO.Request.Blogs;
using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BlogCommentReactions
{
   
    public record UpdateBlogCommentReactionCommand(UpdateBlogCommentReactionRequestDTO model) : IRequest<ServiceResponse>;

}
