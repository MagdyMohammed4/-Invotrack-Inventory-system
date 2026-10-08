using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Domain.Entities
{
    public class Category : BaseEntity
    {
        [Required, MaxLength(100)]
        public string NameAr { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
