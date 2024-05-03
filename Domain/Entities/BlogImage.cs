using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BlogImage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BlogId { get; set; }

        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }  // This navigation property is not necessary for migration, but can be useful for querying related entities in your application


        public string Image { get; set; }
    }
}
