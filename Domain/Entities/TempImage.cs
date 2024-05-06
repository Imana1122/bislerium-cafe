using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TempImage
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
      
        public string Image { get; set; }
    }
}
