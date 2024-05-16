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
    //for creating new blog
    public record CreateBlogCommand(AddBlogRequestDTO BlogModel) :IRequest<ServiceResponse>;
    
}
