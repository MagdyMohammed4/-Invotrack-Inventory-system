using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class StockReportItemDto
    {
        public int Id { get; set; }
        public string DocumentNo { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string MovementTypeName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SourceWarehouseName { get; set; } = string.Empty;
        public string DestinationWarehouseName { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public int BalanceAfter { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
