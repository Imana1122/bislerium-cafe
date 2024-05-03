using Application.Extensions.Identity;
using Domain.Entities;
using Domain.Entities.ActivityTracker;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Blogs
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Tracker> ActivityTracker { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<BlogImage> BlogImages { get; set; }

        public DbSet<BlogComment> BlogComments { get; set; }
       
        public DbSet<BlogReaction> BlogReactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call base method to configure ASP.NET Core Identity entities

            modelBuilder.Entity<BlogComment>()
                .HasKey(bc => new { bc.BlogId, bc.UserId });

            modelBuilder.Entity<BlogReaction>()
               .HasKey(bc => new { bc.BlogId, bc.UserId });
        }


    }
}
