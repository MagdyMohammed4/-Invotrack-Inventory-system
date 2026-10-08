using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class StockValuationExportDto
    {
        public string SKU { get; set; } = string.Empty;
        public string ProductNameAr { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalStock { get; set; }
        public string Unit { get; set; } = "قطعة";
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalCostValue => TotalStock * CostPrice;
        public decimal TotalSaleValue => TotalStock * SalePrice;
    }
}
