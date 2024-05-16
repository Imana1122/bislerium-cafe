using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response.Blogs
{
    //response model for getting all blog images
    public class GetAllBlogImagesQuery:IRequest<IEnumerable<string>>;
    
}
