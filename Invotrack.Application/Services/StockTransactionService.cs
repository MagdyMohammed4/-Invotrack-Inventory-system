using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Enums;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. معالجة إذن الدخول (زيادة الرصيد وتحديث المتوسط المرجح للتكلفة)
        public async Task ProcessStockInAsync(StockInDto dto, string userName)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new Exception("المنتج غير موجود");

            // حساب متوسط التكلفة الجديد (Weighted Average Cost)
            decimal currentTotalValue = product.TotalStock * product.CostPrice;
            decimal newInValue = dto.Quantity * dto.UnitPrice;
            int newTotalStock = product.TotalStock + dto.Quantity;

            product.CostPrice = newTotalStock > 0 ? (currentTotalValue + newInValue) / newTotalStock : dto.UnitPrice;
            product.TotalStock = newTotalStock;

            var movement = new StockMovement
            {
                ProductId = dto.ProductId,
                SourceWarehouseId = dto.WarehouseId,
                BusinessPartnerId = dto.SupplierId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                Type = MovementType.StockIn,
                BalanceAfter = newTotalStock,
                DocumentNo = $"IN-{DateTime.Now:yyyyMMddHHmm}",
                Note = dto.Note,
                CreatedBy = userName
            };

            _unitOfWork.Products.Update(product);
            await _unitOfWork.StockMovements.AddAsync(movement);
            await _unitOfWork.CompleteAsync();
        }

        // 2. معالجة إذن الخروج (خصم الكمية بعد التأكد من كفاية الرصيد)
        public async Task ProcessStockOutAsync(StockOutDto dto, string userName)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new Exception("المنتج غير موجود");

            if (product.TotalStock < dto.Quantity)
            {
                throw new InvalidOperationException("الرصيد المتاح بالمخزن غير كافٍ لإتمام عملية الصرف!");
            }

            product.TotalStock -= dto.Quantity;

            var movement = new StockMovement
            {
                ProductId = dto.ProductId,
                SourceWarehouseId = dto.WarehouseId,
                BusinessPartnerId = dto.DistributorId,
                Quantity = dto.Quantity,
                UnitPrice = product.SalePrice,
                Type = MovementType.StockOut,
                BalanceAfter = product.TotalStock,
                DocumentNo = $"OUT-{DateTime.Now:yyyyMMddHHmm}",
                Note = dto.Note,
                CreatedBy = userName
            };

            _unitOfWork.Products.Update(product);
            await _unitOfWork.StockMovements.AddAsync(movement);
            await _unitOfWork.CompleteAsync();
        }
    }
}
