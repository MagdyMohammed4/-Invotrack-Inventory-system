using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class TransactionReceiptDto
    {
        public string DocumentNo { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Unit { get; set; } = "قطعة";
        public string WarehouseName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost => Quantity * UnitCost;
        public DateTime Date { get; set; }
        public string Note { get; set; } = string.Empty;
        public string HandledBy { get; set; } = string.Empty;
    }
}
