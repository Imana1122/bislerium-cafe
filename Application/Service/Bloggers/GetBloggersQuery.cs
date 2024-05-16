using Application.DTO.Response.Bloggers;
using Application.DTO.Response.Blogs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Bloggers
{
    //for getting all bloggers
    public class GetBloggersQuery : IRequest<IEnumerable<GetBloggerResponseDTO>>
    {

    }
  
}
