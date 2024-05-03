using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Product
{
    public class ProductBase
    {
        [Key]
        public Guid Id { get; set; }   = Guid.NewGuid();
        public string Name { get; set; }
    }
}
