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
    public class ReadNotificationHandler(DataAccess.IDbContextFactory<AppDbContext> contextFactory) : IRequestHandler<ReadNotificationCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
               
                var notification = await dbContext.Notifications.FirstOrDefaultAsync(_ => _.Id.Equals(request.Id), cancellationToken);
                if (notification == null)
                {
                    return GeneralDbResponses.ItemNotFound("Notification");
                }
                else
                {

                    notification.Read = true;
                    dbContext.Notifications.Update(notification);


                    await dbContext.SaveChangesAsync(cancellationToken);


                    return GeneralDbResponses.ItemCreated("Notification");
                }
                
               
            }
            catch (Exception ex)
            {
                return new ServiceResponse(true, ex.Message);

            }
        }
    }
}
