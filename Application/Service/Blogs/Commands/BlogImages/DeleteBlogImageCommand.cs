using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Commands.BlogImages
{
    public record DeleteBlogImageCommand(Guid Id) : IRequest<ServiceResponse>;

}
