using Application.DTO.Response.Blogs;
using Application.DTO.Response.Dashboard;
using Application.Extensions.Identity;
using Application.Service.Blogs.Queries.Blogs;
using Application.Service.Dashboard;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.Dashboard
{
    public class GetDashboardDataHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetDashboardDataQuery, DashboardDataDTO>
    {
        public async Task<DashboardDataDTO> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            using var dbContext = contextFactory.CreateDbContext();

           if(request.month != null)
            {
                var blogsCount = await dbContext.Blogs
               .Where(blog => blog.CreatedAt.Month == request.month)
               .AsNoTracking()
               .CountAsync(cancellationToken);

                var reactions = await dbContext.BlogReactions
                    .Where(reaction => reaction.CreatedAt.Month == request.month)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                var upvotesCount = reactions.Where(_=>_.IsUpvote).Count() ;

                var downvotesCount = reactions.Where(_ => _.IsDownvote).Count();


                var commentsCount = await dbContext.BlogComments
                    .Where(comment => comment.CreatedAt.Month == request.month == true)
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

                // Create a DashboardDataDTO instance and assign the counts
                var dashboardData = new DashboardDataDTO
                {
                    BlogsCount = blogsCount,
                    UpvotesCount = upvotesCount,
                    DownvotesCount = downvotesCount,
                    CommentsCount = commentsCount
                };

                return dashboardData;
            }
            else
            {
                var blogsCount = await dbContext.Blogs
               .AsNoTracking()
               .CountAsync(cancellationToken);

                var reactions = await dbContext.BlogReactions
                    .AsNoTracking()
                   .ToListAsync(cancellationToken);

                var upvotesCount = reactions.Where(_ => _.IsUpvote).Count();

                var downvotesCount = reactions.Where(_ => _.IsDownvote).Count();

              

                var commentsCount = await dbContext.BlogComments
                    .AsNoTracking()
                    .CountAsync(cancellationToken);
                // Create a DashboardDataDTO instance and assign the counts
                var dashboardData = new DashboardDataDTO
                {
                    BlogsCount = blogsCount,
                    UpvotesCount = upvotesCount,
                    DownvotesCount = downvotesCount,
                    CommentsCount = commentsCount
                };

                return dashboardData;
            }

          
        }
    }

}
