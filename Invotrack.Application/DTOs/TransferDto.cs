using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class TransferDto
    {
        [Required(ErrorMessage = "اختر المنتج المراد نقله")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "اختر المخزن المصدر")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "اختر المخزن الوجهة")]
        public int DestinationWarehouseId { get; set; }

        [Required(ErrorMessage = "أدخل الكمية المحولة"), Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من 0")]
        public int Quantity { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}
