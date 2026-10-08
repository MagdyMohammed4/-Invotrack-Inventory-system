using System;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Enums;
using Invotrack.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Invotrack.UI.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IStockTransactionService _transactionService;

        public TransactionsController(IUnitOfWork unitOfWork, IProductService productService,
            IWarehouseService warehouseService, IStockTransactionService stockTransactionService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _warehouseService = warehouseService;
            _transactionService = stockTransactionService;
            
        }

      
        public async Task<IActionResult> StockIn()
        {
            ViewBag.Products = await _unitOfWork.Products.GetAllAsync();
            ViewBag.Warehouses = await _unitOfWork.Warehouses.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockIn(StockInDto dto)
        {
            if (ModelState.IsValid)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                if (product != null)
                {
                    decimal oldTotalValue = product.TotalStock * product.CostPrice;
                    decimal newInValue = dto.Quantity * dto.UnitPrice;
                    int newTotalStock = product.TotalStock + dto.Quantity;

                    product.CostPrice = (oldTotalValue + newInValue) / newTotalStock;
                    product.TotalStock = newTotalStock;

                    string docNo = $"IN-{DateTime.Now:yyyyMMddHHmm}";
                    var movement = new StockMovement
                    {
                        ProductId = dto.ProductId,
                        SourceWarehouseId = dto.WarehouseId,
                        Quantity = dto.Quantity,
                        UnitPrice = dto.UnitPrice,
                        Type = MovementType.StockIn,
                        BalanceAfter = newTotalStock,
                        DocumentNo = docNo,
                        Note = dto.Note,
                        CreatedBy = User.Identity?.Name ?? "System"
                    };

                    _unitOfWork.Products.Update(product);
                    await _unitOfWork.StockMovements.AddAsync(movement);
                    await _unitOfWork.CompleteAsync();

                   
                    
                }
            }
            ViewBag.Products = await _unitOfWork.Products.GetAllAsync();
            ViewBag.Warehouses = await _unitOfWork.Warehouses.GetAllAsync();
            return View(dto);
        }

       
        public async Task<IActionResult> StockOut()
        {
            ViewBag.Products = await _unitOfWork.Products.GetAllAsync();
            ViewBag.Warehouses = await _unitOfWork.Warehouses.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOut(StockOutDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product != null)
            {
                if (product.TotalStock < dto.Quantity)
                {
                    ModelState.AddModelError("", "الكمية المطلوبة أكبر من الرصيد المتاح بالمخزن!");
                }
                else if (ModelState.IsValid)
                {
                    product.TotalStock -= dto.Quantity;

                    string docNo = $"OUT-{DateTime.Now:yyyyMMddHHmm}";
                    var movement = new StockMovement
                    {
                        ProductId = dto.ProductId,
                        SourceWarehouseId = dto.WarehouseId,
                        Quantity = dto.Quantity,
                        UnitPrice = product.SalePrice,
                        Type = MovementType.StockOut,
                        BalanceAfter = product.TotalStock,
                        DocumentNo = docNo,
                        Note = dto.Note,
                        CreatedBy = User.Identity?.Name ?? "System"
                    };

                    _unitOfWork.Products.Update(product);
                    await _unitOfWork.StockMovements.AddAsync(movement);
                    await _unitOfWork.CompleteAsync();

                   
                    
                }
            }

            ViewBag.Products = await _unitOfWork.Products.GetAllAsync();
            ViewBag.Warehouses = await _unitOfWork.Warehouses.GetAllAsync();
            return View(dto);
        }



        public async Task<IActionResult> Adjustment()
        {
            await PopulateDropdowns();
            return View();
        }

        // 2. معالجة التسوية الجردية (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjustment(StockAdjustmentDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _productService.GetProductByIdAsync(dto.ProductId);
                    if (product != null)
                    {
                        int diff = dto.ActualQuantity - product.TotalStock;

                        if (diff > 0)
                        {
                            var inDto = new StockInDto
                            {
                                ProductId = dto.ProductId,
                                WarehouseId = dto.WarehouseId,
                                Quantity = diff,
                                UnitPrice = product.CostPrice,
                                Note = $"تسوية جردية (فائض): {dto.Reason}"
                            };
                            await _transactionService.ProcessStockInAsync(inDto, User.Identity?.Name ?? "System");
                        }
                        else if (diff < 0)
                        {
                            var outDto = new StockOutDto
                            {
                                ProductId = dto.ProductId,
                                WarehouseId = dto.WarehouseId,
                                Quantity = Math.Abs(diff),
                                Note = $"تسوية جردية (عجز): {dto.Reason}"
                            };
                            await _transactionService.ProcessStockOutAsync(outDto, User.Identity?.Name ?? "System");
                        }

                        TempData["Success"] = "تم تسجيل التسوية الجردية وتعديل رصيد المنتج بنجاح";
                        return RedirectToAction("StockMovements", "Reports");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // 🟢 هام جداً: إعادة ملء القوائم المنسدلة عند حدوث خطأ حتى لا تصبح null
            await PopulateDropdowns();
            return View(dto);
        }
        // --- المساعد لملء القوائم المنسدلة ---
        private async Task PopulateDropdowns(PartnerType? partnerType = null)
        {
            var productsResult = await _productService.GetProductsPagedAsync(null, 1, 1000);
            var warehouses = await _warehouseService.GetAllWarehousesAsync();

            ViewBag.Products = new SelectList(productsResult.Items, "Id", "NameAr");
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");

            
        }


    }
}