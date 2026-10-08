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
    public class TransferService : ITransferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessTransferAsync(TransferDto dto, string userName)
        {
            if (dto.SourceWarehouseId == dto.DestinationWarehouseId)
            {
                throw new InvalidOperationException("لا يمكن التحويل لنفس المخزن المصدر!");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new Exception("المنتج غير موجود");

            if (product.TotalStock < dto.Quantity)
            {
                throw new InvalidOperationException("الرصيد المتاح غير كافٍ لإتمام عملية النقل!");
            }

            // تسجيل حركة النقل مزدوجة الأثر (نقل بين المخازن)
            var movement = new StockMovement
            {
                ProductId = dto.ProductId,
                SourceWarehouseId = dto.SourceWarehouseId,
                DestinationWarehouseId = dto.DestinationWarehouseId,
                Quantity = dto.Quantity,
                UnitPrice = product.CostPrice,
                Type = MovementType.Transfer,
                BalanceAfter = product.TotalStock, // الرصيد المتبقي الإجمالي
                DocumentNo = $"TRF-{DateTime.Now:yyyyMMddHHmm}",
                Note = dto.Note,
                CreatedBy = userName
            };

            await _unitOfWork.StockMovements.AddAsync(movement);
            await _unitOfWork.CompleteAsync();
        }
    }
}
