using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class LowStockProductDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Unit { get; set; } = "قطعة";
        public int TotalStock { get; set; }
        public int MinReorderQuantity { get; set; }
        public int ShortageQuantity => MinReorderQuantity - TotalStock; // مقدار العجز
    }
}
