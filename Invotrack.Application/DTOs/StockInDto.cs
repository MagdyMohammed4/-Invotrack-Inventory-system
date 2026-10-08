using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class StockInDto
    {
        [Required(ErrorMessage = "اختر المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "اختر المخزن المستلم")]
        public int WarehouseId { get; set; }

        public int? SupplierId { get; set; }

        [Required(ErrorMessage = "أدخل الكمية المضافة"), Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "أدخل سعر تكلفة الوحدة"), Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون موجباً")]
        public decimal UnitPrice { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}
