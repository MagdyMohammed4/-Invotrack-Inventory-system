using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class StockAdjustmentDto
    {
        [Required(ErrorMessage = "اختر الصنف المطلوب تسويته")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "اختر المستودع")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "أدخل الرصيد الفعلي في الجرد")]
        [Range(0, int.MaxValue, ErrorMessage = "الرصيد لا يمكن أن يكون بالسالب")]
        public int ActualQuantity { get; set; }

        [Required(ErrorMessage = "سبب التسوية الجردية مطلوب")]
        public string Reason { get; set; } = string.Empty;
    }
}
