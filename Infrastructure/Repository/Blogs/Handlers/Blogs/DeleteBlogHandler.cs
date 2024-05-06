using Application.DTO.Response;
using Application.Service.Blogs.Commands.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    public class DeleteBlogHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<DeleteBlogCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                var data = await dbContext.Blogs
                    .Include(_ => _.Reactions) // Include related reactions
                    .Include(_ => _.Comments) // Include related reactions
                    .Include(_ => _.Images) // Include related reactions
                    .FirstOrDefaultAsync(_ => _.Id.Equals(request.Id), cancellationToken: cancellationToken);
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("BLog");
                }
                if (data.Reactions != null)
                {
                    foreach (var item in data.Reactions)
                    {
                        dbContext.BlogReactions.Remove(item);
                    }
                }

                if (data.Images != null)
                {
                    foreach (var item in data.Images)
                    {
                        dbContext.BlogImages.Remove(item);
                    }
                }

                if (data.Comments != null)
                {
                    foreach (var item in data.Comments)
                    {
                        var comment = await dbContext.BlogComments
                            .Include(comment => comment.Reactions) // Include related reactions
                            .FirstOrDefaultAsync(_ => _.BlogId.ToString().ToLower().Equals(item.BlogId.ToString().ToLower()) && _.UserId.ToString().ToLower().Equals(item.UserId.ToString().ToLower()), cancellationToken);

                        if (comment != null && comment.Reactions != null)
                        {
                            foreach (var reaction in comment.Reactions)
                            {
                                dbContext.BlogCommentReactions.Remove(reaction);
                            }
                        }

                        dbContext.BlogComments.Remove(item);
                    }
                }

                dbContext.Blogs.Remove(data);
                await dbContext.SaveChangesAsync(cancellationToken);

                var history = new UserHistory
                {
                    UserId = data.UserId,
                    Content = data.Title + " is deleted."

                };
                dbContext.Histories.Add(history);
                await dbContext.SaveChangesAsync(cancellationToken);

                var blogImages = await dbContext.BlogImages.FirstAsync(_=>_.BlogId.ToString().ToLower().Equals(request.Id.ToString().ToLower()),cancellationToken:cancellationToken);
                // Assuming dbContext is your instance of DbContext and blogImages is your collection
                dbContext.BlogImages.RemoveRange(blogImages);
                await dbContext.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemDelete(data.Title);
            }catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
