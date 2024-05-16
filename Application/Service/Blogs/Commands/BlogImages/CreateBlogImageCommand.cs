using Application.DTO.Request.Blogs;
using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Commands.BlogImages
{
    //for creating blog image
    public record CreateBlogImageCommand(AddBlogImageByBlogIdRequestDTO imageModel): IRequest<ServiceResponse>;

}
