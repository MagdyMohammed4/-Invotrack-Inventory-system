using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Invotrack.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invotrack.UI.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IStockReportService _reportService;
        private readonly IWarehouseService _warehouseService;

        public ReportsController(
            IProductService productService,
            IStockReportService reportService,
            IWarehouseService warehouseService)
        {
            _productService = productService;
            _reportService = reportService;
            _warehouseService = warehouseService;
        }

        
        public async Task<IActionResult> Index()
        {
            var productsResult = await _productService.GetProductsPagedAsync(null, 1, 10000);
            var products = productsResult.Items.ToList();

            var totalCost = products.Sum(p => p.TotalStock * p.CostPrice);
            var totalSale = products.Sum(p => p.TotalStock * p.SalePrice);

            var model = new ReportsDashboardViewModel
            {
                TotalStockQuantity = products.Sum(p => p.TotalStock),
                MinimumRequiredStock = products.Sum(p => p.MinReorderQuantity),
                LowStockCount = products.Count(p => p.TotalStock <= p.MinReorderQuantity && p.TotalStock > 0),
                OutOfStockCount = products.Count(p => p.TotalStock == 0),
                InStockCount = products.Count(p => p.TotalStock > p.MinReorderQuantity),

                TotalStockCostValue = totalCost,
                TotalStockSaleValue = totalSale,
                ExpectedProfitMargin = totalSale - totalCost,

                TopWarehouseName = "المستودع الرئيسي",
                TopWarehouseStock = products.Sum(p => p.TotalStock)
            };

            return View(model);
        }

        
        public async Task<IActionResult> StockInReport(DateTime? fromDate, DateTime? toDate, int? warehouseId)
        {
            var filter = new StockReportFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                WarehouseId = warehouseId,
                MovementType = MovementType.StockIn
            };

            var movements = await _reportService.GetMovementsReportAsync(filter);

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.TotalInQuantity = movements.Sum(m => m.Quantity);

            return View(movements);
        }

       
        public async Task<IActionResult> StockOutReport(DateTime? fromDate, DateTime? toDate, int? warehouseId)
        {
            var filter = new StockReportFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                WarehouseId = warehouseId,
                MovementType = MovementType.StockOut
            };

            var movements = await _reportService.GetMovementsReportAsync(filter);

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.TotalOutQuantity = movements.Sum(m => m.Quantity);

            return View(movements);
        }


      
        public async Task<IActionResult> StockMovements(StockReportFilterDto filter)
        {
            var reports = await _reportService.GetMovementsReportAsync(filter);
            return View(reports);
        }

       
        public async Task<IActionResult> CurrentStockReport()
        {
            var productsResult = await _productService.GetProductsPagedAsync(null, 1, 10000);
            return View(productsResult.Items);
        }

       
        public async Task<IActionResult> FinancialValuation()
        {
            var productsResult = await _productService.GetProductsPagedAsync(null, 1, 10000);
            return View(productsResult.Items);
        }

       
        public async Task<IActionResult> ExportCenter()
        {
            var productsResult = await _productService.GetProductsPagedAsync(null, 1, 10000);
            return View(productsResult.Items);
        }

       
        [HttpGet]
        public async Task<IActionResult> ExportProductsCsv()
        {
            var productsResult = await _productService.GetProductsPagedAsync(null, 1, 10000);
            var products = productsResult.Items;

            var builder = new StringBuilder();
            
            builder.AppendLine("SKU,اسم الصنف,الفئة,الرصيد المتاح,سعر التكلفة,سعر البيع,إجمالي التكلفة");

            foreach (var item in products)
            {
                var totalCost = item.TotalStock * item.CostPrice;
                builder.AppendLine($"\"{item.SKU}\",\"{item.NameAr}\",\"{item.CategoryName}\",{item.TotalStock},{item.CostPrice},{item.SalePrice},{totalCost}");
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(builder.ToString())).ToArray();
            return File(bytes, "text/csv; charset=utf-8", $"Products_Report_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        
        [HttpGet]
        public async Task<IActionResult> ExportProductsPdf()
        {
            
            return RedirectToAction(nameof(CurrentStockReport));
        }
    }
}