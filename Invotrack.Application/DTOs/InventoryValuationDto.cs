using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class InventoryValuationDto
    {
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalCostValue => TotalQuantity * CostPrice;
        public decimal TotalSaleValue => TotalQuantity * SalePrice;
        public decimal ExpectedProfit => TotalSaleValue - TotalCostValue;
    }
}
