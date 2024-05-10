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
    //Handler to get total cumulative count of blogs, upvotes, downvotes and comments for dashboard
    public class GetDashboardDataHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<GetDashboardDataQuery, DashboardDataDTO>
    {
        public async Task<DashboardDataDTO> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the database context using the factory
            using var dbContext = contextFactory.CreateDbContext();

            // Check if a specific month is specified in the request
            if (request.month != null)
            {
                // Retrieve the count of blogs updated in the specified month
                var blogsCount = await dbContext.Blogs
                    .Where(blog => blog.UpdatedAt.Month == request.month)
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

                // Retrieve reactions for blogs updated in the specified month
                var reactions = await dbContext.BlogReactions
                    .Where(reaction => reaction.UpdatedAt.Month == request.month)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                // Calculate the count of upvotes and downvotes from the reactions
                var upvotesCount = reactions.Where(_ => _.IsUpvote).Count();
                var downvotesCount = reactions.Where(_ => _.IsDownvote).Count();

                // Retrieve the count of comments made in the specified month
                var commentsCount = await dbContext.BlogComments
                    .Where(comment => comment.UpdatedAt.Month == request.month)
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

                // Return the dashboard data
                return dashboardData;
            }
            else
            {
                // If no specific month is specified, retrieve counts for all blogs, reactions, and comments
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

                // Return the dashboard data
                return dashboardData;
            }
        }

    }

}
