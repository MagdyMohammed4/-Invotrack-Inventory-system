using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        
        public string? SKU { get; set; }

        [Required(ErrorMessage = "اسم الصنف بالعربية مطلوب")]
        public string NameAr { get; set; } = string.Empty;

        public string? NameEn { get; set; }

        [Required(ErrorMessage = "اختر التصنيف")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public string Unit { get; set; } = "قطعة";

        [Range(0, double.MaxValue, ErrorMessage = "سعر التكلفة يجب أن يكون موجباً")]
        public decimal CostPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "سعر البيع يجب أن يكون موجباً")]
        public decimal SalePrice { get; set; }

        public int TotalStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "حد إشعار الطلب يجب أن يكون موجباً")]
        public int MinReorderQuantity { get; set; }

        public bool IsLowStock => TotalStock <= MinReorderQuantity;
    }
}
