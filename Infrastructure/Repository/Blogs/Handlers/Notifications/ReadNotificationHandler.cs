using Application.DTO.Response;
using Application.Extensions.Identity;
using Application.Service.Blogs.Commands.BlogReactions;
using Application.Service.Notifications;
using Domain.Entities;
using Infrastructure.DataAccess;
using Infrastructure.Repository.Products;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blogs.Handlers.Notifications
{
    //Handle to read notification by notification id
    public class ReadNotificationHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<ReadNotificationCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create a new instance of the database context using the factory
                using var dbContext = contextFactory.CreateDbContext();

                // Find the notification by its ID
                var notification = await dbContext.Notifications.FirstOrDefaultAsync(_ => _.Id.Equals(request.Id), cancellationToken);

                // If notification not found, return a response indicating it wasn't found
                if (notification == null)
                {
                    return GeneralDbResponses.ItemNotFound("Notification");
                }
                else
                {
                    // Mark the notification as read
                    notification.Read = true;
                    dbContext.Notifications.Update(notification);

                    // Save changes to the database
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Return a response indicating successful update of the notification
                    return GeneralDbResponses.ItemCreated("Notification");
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs during the execution, return a response with an error message
                return new ServiceResponse(true, ex.Message);
            }
        }

    }
}
