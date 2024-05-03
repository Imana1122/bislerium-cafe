using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities.Product
{
    public class Category:ProductBase
    {
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; } = null;

    }
}
