using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Blog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public virtual ICollection<BlogImage> Images { get; set; } = [];
        public virtual ICollection<BlogComment> Comments { get; set; } = [];
        public virtual ICollection<BlogReaction> Reactions { get; set; } = [];



    }
}
