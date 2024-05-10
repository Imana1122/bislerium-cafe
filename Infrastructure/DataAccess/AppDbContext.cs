using Application.Extensions.Identity;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Represents the database context for the application.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Gets or sets the DbSet for blogs.
        /// </summary>
        public DbSet<Blog> Blogs { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for blog images.
        /// </summary>
        public DbSet<BlogImage> BlogImages { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for blog comments.
        /// </summary>
        public DbSet<BlogComment> BlogComments { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for blog reactions.
        /// </summary>
        public DbSet<BlogReaction> BlogReactions { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for user activity histories.
        /// </summary>
        public DbSet<History> Histories { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for notifications.
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for blog comment reactions.
        /// </summary>
        public DbSet<BlogCommentReaction> BlogCommentReactions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures the model relationships and behavior for the database context.
        /// </summary>
        /// <param name="modelBuilder">The model builder instance to use for configuration.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call base method to configure ASP.NET Core Identity entities

            // Configure composite primary keys for certain entities
            modelBuilder.Entity<BlogComment>().HasKey(bc => new { bc.BlogId, bc.UserId });
            modelBuilder.Entity<BlogReaction>().HasKey(bc => new { bc.BlogId, bc.UserId });
            modelBuilder.Entity<BlogCommentReaction>().HasKey(bc => new { bc.BlogId, bc.UserId, bc.CommentUserId });
        }
    }
}
