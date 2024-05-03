using Application.DTO.Response.Blogs;
using Application.Service.Blogs.Queries.BlogComments;
using Application.Service.Blogs.Queries.Blogs;
using Infrastructure.DataAccess.Blogs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.BlogComments
{
    public class GetAllBlogCommentsByBlogId(DataAccess.Blogs.IDbContextFactory<AppDbContext> _contextFactory) : IRequestHandler<GetAllBlogCommentsByBlogIdQuery, IEnumerable<GetBlogCommentResponseDTO>>

    {
      public async Task<IEnumerable<GetBlogCommentResponseDTO>> Handle(GetAllBlogCommentsByBlogIdQuery request, CancellationToken cancellationToken)
    {
        using var dbContext = _contextFactory.CreateDbContext();
        
        var comments = await dbContext.BlogComments
            .Where(comment => comment.BlogId.Equals(request.BlogId) )// Filter comments by BlogId
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Adapt blog comment entities to DTOs
        var commentDTOs = comments.Select(comment => comment.Adapt<GetBlogCommentResponseDTO>());

        return commentDTOs;
    }
    }
}
