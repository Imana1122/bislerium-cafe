using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class BlogReaction
    {
        [Key, Column(Order = 0)]

        public Guid BlogId { get; set; }

        [Key, Column(Order = 1)]

        public Guid UserId { get; set; }


        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }  // This navigation property is not necessary for migration, but can be useful for querying related entities in your application

        public string Reaction { get; set; }

        [NotMapped] // Not mapped to the database
        public bool IsUpvote => Reaction == "Upvote";

        [NotMapped] // Not mapped to the database
        public bool IsDownvote => Reaction == "Downvote";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
