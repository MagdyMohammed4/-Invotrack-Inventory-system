using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Domain.Entities
{
    public class Product : BaseEntity
    {
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string NameAr { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required, MaxLength(20)]
        public string Unit { get; set; } = "قطعة";
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int TotalStock { get; set; }
        public int MinReorderQuantity { get; set; }
    }
}
