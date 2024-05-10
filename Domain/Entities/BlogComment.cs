using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BlogComment
    {
        [Key, Column(Order = 0)]
        
        public Guid BlogId { get; set; }

        [Key, Column(Order = 1)]

        public Guid UserId { get; set; }

        public string Comment { get; set; }

        // Navigation properties (optional)
        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }

       
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public virtual ICollection<BlogCommentReaction> Reactions { get; set; } = null;


    }
}
