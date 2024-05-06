using Application.DTO.Response.Blogs;
using Application.Extensions.Identity;
using Application.Service.Blogs.Queries.BlogComments;
using Application.Service.Blogs.Queries.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    public class GetAllBlogCommentsByBlogId(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<GetAllBlogCommentsByBlogIdQuery, IEnumerable<GetBlogCommentResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogCommentResponseDTO>> Handle(GetAllBlogCommentsByBlogIdQuery request, CancellationToken cancellationToken)
        {
           
            using var dbContext = contextFactory.CreateDbContext();

            var comments = await dbContext.BlogComments
                .Where(comment => comment.BlogId.Equals(request.BlogId))
                .Include(comment => comment.Reactions) // Include related reactions
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var commentDTOs = new List<GetBlogCommentResponseDTO>();

            foreach (var comment in comments)
            {
                var user = await userManager.FindByIdAsync(comment.UserId.ToString());

                // Map comment entity to DTO
                var commentDTO = comment.Adapt<GetBlogCommentResponseDTO>();
                commentDTO.User = user; // Set the UserName property
                commentDTO.UpvoteCount = comment.Reactions?.Count(r => r.IsUpvote) ?? 0;
                commentDTO.DownvoteCount = comment.Reactions?.Count(r => r.IsDownvote) ?? 0;

                if (request.UserId != null)
                {
                    BlogCommentReaction reaction = comment.Reactions
                        .Where(_ => _.UserId.Equals(request.UserId) && _.BlogId.Equals(comment.BlogId) && _.CommentUserId.Equals(comment.UserId))
                        .FirstOrDefault();

                    if (reaction != null)
                    {
                        commentDTO.DownvotedStatus = reaction.IsDownvote == true;
                        commentDTO.UpvotedStatus = reaction.IsUpvote == true;
                    }
                    else
                    {
                        // Handle the case when no matching BlogReaction is found
                        commentDTO.UpvotedStatus = false;
                        commentDTO.DownvotedStatus = false;
                    }
                }
                commentDTOs.Add(commentDTO);
            }

            return commentDTOs;
        }
    }
}
