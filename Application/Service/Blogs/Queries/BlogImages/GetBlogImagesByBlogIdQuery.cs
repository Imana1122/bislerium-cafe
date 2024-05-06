using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Queries.BlogImages
{
    public record GetBlogImagesByBlogIdQuery(Guid Id): IRequest<IEnumerable<BlogImage>>;
    
}
