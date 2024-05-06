using Application.Extensions.Email;
using Application.Extensions.Identity;
using Application.Interface.Emails;
using Application.Interface.Identity;
using Application.Service.BlogCommentReactions;
using Infrastructure.DataAccess;
using Infrastructure.Repository;
using Infrastructure.Repository.Blogs.Handlers;
using Infrastructure.Repository.Products.Handlers.Blogs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(config.GetConnectionString("Default")), ServiceLifetime.Scoped);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();

            services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(5));
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdministrationPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin", "Manager");
                });

                options.AddPolicy("UserPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Blogger");
                });
            });
            services.AddCascadingAuthenticationState();
            services.AddScoped<IAccount, Account>();
            services.AddScoped<IEmail, Email>();

            var emailConfig = config.GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
         
            services.AddSingleton(emailConfig);

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateBlogHandler).Assembly));

            services.AddScoped<DataAccess.IDbContextFactory<AppDbContext>, DbContextFactory<AppDbContext>>();


            return services;


        }
    }
}
