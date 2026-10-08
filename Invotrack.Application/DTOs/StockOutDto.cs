using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class StockOutDto
    {
        [Required(ErrorMessage = "اختر المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "اختر المخزن المنصرف منه")]
        public int WarehouseId { get; set; }

        public int? DistributorId { get; set; }

        [Required(ErrorMessage = "أدخل الكمية المنصرفة"), Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من 0")]
        public int Quantity { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}
