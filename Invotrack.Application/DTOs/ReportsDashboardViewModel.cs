using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class ReportsDashboardViewModel
    {
        
        public int TotalStockQuantity { get; set; }
        public int MinimumRequiredStock { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int InStockCount { get; set; }

       
        public decimal TotalStockCostValue { get; set; }     
        public decimal TotalStockSaleValue { get; set; }    
        public decimal ExpectedProfitMargin { get; set; }     

        
        public string TopWarehouseName { get; set; } = string.Empty;
        public int TopWarehouseStock { get; set; }
    }
}
