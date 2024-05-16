using Application.DTO.Response;
using Application.Service.Blogs.Commands.Blogs;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Products.Handlers.Categories
{
    // Handler class responsible for deleting a blog

    public class DeleteBlogHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<DeleteBlogCommand, ServiceResponse>
    {
        // Method to handle the deletion of a blog
        public async Task<ServiceResponse> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Using statement to ensure proper disposal of DbContext
                using var dbContext = contextFactory.CreateDbContext();

                // Retrieving the blog to be deleted including related entities
                var data = await dbContext.Blogs
                    .Include(_ => _.Reactions) // Include related reactions
                    .Include(_ => _.Comments) // Include related comments
                    .Include(_ => _.Images) // Include related images
                    .FirstOrDefaultAsync(_ => _.Id.Equals(request.Id), cancellationToken: cancellationToken);

                // If blog not found, return not found response
                if (data == null)
                {
                    return GeneralDbResponses.ItemNotFound("Blog");
                }

                // Removing reactions related to the blog
                if (data.Reactions != null)
                {
                    foreach (var item in data.Reactions)
                    {
                        dbContext.BlogReactions.Remove(item);
                    }
                }

                // Removing images related to the blog
                if (data.Images != null)
                {
                    foreach (var item in data.Images)
                    {
                        dbContext.BlogImages.Remove(item);
                    }
                }

                // Removing comments related to the blog
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



                // Deleting blog images from the database
                var blogImages = await dbContext.BlogImages.FirstAsync(_ => _.BlogId.ToString().ToLower().Equals(request.Id.ToString().ToLower()), cancellationToken: cancellationToken);
                dbContext.BlogImages.RemoveRange(blogImages);

                // Removing the blog itself
                dbContext.Blogs.Remove(data);
                await dbContext.SaveChangesAsync(cancellationToken);

                // Adding deletion history for the blog
                var history = new History
                {
                    UserId = data.UserId,
                    Content = data.Title + " is deleted."
                };
                dbContext.Histories.Add(history);

                await dbContext.SaveChangesAsync(cancellationToken);

                // Returning success response with deleted blog title
                return GeneralDbResponses.ItemDelete("Blog");
            }
            catch (Exception ex)
            {
                // Returning error response if an exception occurs
                return new ServiceResponse(false, ex.Message);
            }
        }
    }
}
