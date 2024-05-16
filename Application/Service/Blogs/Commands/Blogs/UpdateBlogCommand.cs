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
    //for updating existing blog
    public record UpdateBlogCommand(UpdateBlogRequestDTO BlogModel) :IRequest<ServiceResponse>;
   
}
