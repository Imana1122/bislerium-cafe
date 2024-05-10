using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class BlogCommentReaction
    {
        [Key, Column(Order = 0)]
        public Guid BlogId { get; set; }

        [Key, Column(Order = 1)]
        public Guid UserId { get; set; }

        [Key, Column(Order = 2)]
        public Guid CommentUserId { get; set; }

        public string Reaction { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property to BlogComment entity
        [ForeignKey("BlogId,CommentUserId")] // Define the composite foreign key
        public BlogComment BlogComment { get; set; }

        [NotMapped] // Not mapped to the database
        public bool IsUpvote => Reaction == "Upvote";

        [NotMapped] // Not mapped to the database
        public bool IsDownvote => Reaction == "Downvote";
    }
}
