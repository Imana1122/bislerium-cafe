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
    //Handler to get all blog comments by blog id
    public class GetAllBlogCommentsByBlogId(DataAccess.IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IRequestHandler<GetAllBlogCommentsByBlogIdQuery, IEnumerable<GetBlogCommentResponseDTO>>
    {
        public async Task<IEnumerable<GetBlogCommentResponseDTO>> Handle(GetAllBlogCommentsByBlogIdQuery request, CancellationToken cancellationToken)
        {
            // Create a new instance of the database context
            using var dbContext = contextFactory.CreateDbContext();

            // Retrieve all comments for the specified blog ID, including reactions
            var comments = await dbContext.BlogComments
                .Where(comment => comment.BlogId.Equals(request.BlogId))
                .Include(comment => comment.Reactions) // Include related reactions
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Initialize a list to store comment DTOs
            var commentDTOs = new List<GetBlogCommentResponseDTO>();

            // Iterate through each comment retrieved from the database
            foreach (var comment in comments)
            {
                // Retrieve the user associated with the comment
                var user = await userManager.FindByIdAsync(comment.UserId.ToString());

                // Map the comment entity to a DTO
                var commentDTO = comment.Adapt<GetBlogCommentResponseDTO>();
                commentDTO.User = user; // Set the UserName property

                // Calculate the upvote count for the comment
                commentDTO.UpvoteCount = comment.Reactions?.Count(r => r.IsUpvote) ?? 0;

                // Calculate the downvote count for the comment
                commentDTO.DownvoteCount = comment.Reactions?.Count(r => r.IsDownvote) ?? 0;

                // Check if a user is logged in
                if (request.UserId != null)
                {
                    // Retrieve the reaction of the logged-in user for the comment
                    BlogCommentReaction reaction = comment.Reactions
                        .Where(_ => _.UserId.Equals(request.UserId) && _.BlogId.Equals(comment.BlogId) && _.CommentUserId.Equals(comment.UserId))
                        .FirstOrDefault();

                    // Set the upvoted and downvoted status based on the user's reaction
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

                // Add the comment DTO to the list
                commentDTOs.Add(commentDTO);
            }

            // Return the list of comment DTOs
            return commentDTOs;
        }

    }
}
