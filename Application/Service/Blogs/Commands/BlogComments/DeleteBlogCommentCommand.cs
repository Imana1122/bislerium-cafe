using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Commands.BlogComments
{
    //for deleting blog comment
    public record DeleteBlogCommentCommand(Guid UserId, Guid BlogId) : IRequest<ServiceResponse>;
    
}
