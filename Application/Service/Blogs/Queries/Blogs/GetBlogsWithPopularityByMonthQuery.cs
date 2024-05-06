﻿using Application.DTO.Response.Blogs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Blogs.Queries.Blogs
{
    public record GetBlogsWithPopularityByMonthQuery(int month) : IRequest<IEnumerable<GetBlogsResponseDTO>>;

}
