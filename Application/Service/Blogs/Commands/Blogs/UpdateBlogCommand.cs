using Application.DTO.Request.Blogs;
using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Commands.Blogs
{
    public record UpdateBlogCommand(UpdateBlogRequestDTO BlogModel) :IRequest<ServiceResponse>;
   
}
