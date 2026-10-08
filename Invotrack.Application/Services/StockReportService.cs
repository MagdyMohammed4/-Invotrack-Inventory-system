using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Enums;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class StockReportService : IStockReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StockReportItemDto>> GetMovementsReportAsync(StockReportFilterDto filter)
        {
            // 🟢 1. جلب البيانات الأساسية للـ Lookups لتسريع الأداء
            var products = (await _unitOfWork.Products.GetAllAsync()).ToDictionary(p => p.Id, p => p.NameAr);
            var warehouses = (await _unitOfWork.Warehouses.GetAllAsync()).ToDictionary(w => w.Id, w => w.Name);
            var partners = (await _unitOfWork.BusinessPartners.GetAllAsync()).ToDictionary(p => p.Id, p => p.Name);

            // 🟢 2. جلب الحركات وتطبيق الفلترة
            var movements = await _unitOfWork.StockMovements.GetAllAsync();
            var query = movements.AsEnumerable();

            if (filter.ProductId.HasValue)
                query = query.Where(m => m.ProductId == filter.ProductId.Value);

            if (filter.WarehouseId.HasValue)
            {
                query = query.Where(m =>
                    m.SourceWarehouseId == filter.WarehouseId.Value ||
                    m.DestinationWarehouseId == filter.WarehouseId.Value);
            }

            if (filter.MovementType.HasValue)
                query = query.Where(m => m.Type == filter.MovementType.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(m => m.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(m => m.CreatedAt <= filter.ToDate.Value);

            // 🟢 3. تنفيذ الـ Mapping بأمان تام بداخل الـ Memory
            return query
                .OrderByDescending(m => m.CreatedAt)
                .Select(m =>
                {
                    string pName = products.TryGetValue(m.ProductId, out var nameP) ? nameP : "-";
                    string swName = warehouses.TryGetValue(m.SourceWarehouseId, out var nameSW) ? nameSW : "-";

                    string dwName = "-";
                    if (m.DestinationWarehouseId.HasValue && warehouses.TryGetValue(m.DestinationWarehouseId.Value, out var nameDW))
                    {
                        dwName = nameDW;
                    }

                    string partnerName = "-";
                    if (m.BusinessPartnerId.HasValue && partners.TryGetValue(m.BusinessPartnerId.Value, out var namePart))
                    {
                        partnerName = namePart;
                    }

                    string movementTypeName = m.Type switch
                    {
                        MovementType.StockIn => "إذن دخول (وارد)",
                        MovementType.StockOut => "إذن خروج (صادر)",
                        MovementType.Transfer => "نقل بين المخازن",
                        _ => "غير محدد"
                    };

                    return new StockReportItemDto
                    {
                        Id = m.Id,
                        DocumentNo = m.DocumentNo,
                        Date = m.CreatedAt,
                        MovementTypeName = movementTypeName,
                        ProductName = pName,
                        SourceWarehouseName = swName,
                        DestinationWarehouseName = dwName,
                        PartnerName = partnerName,
                        Quantity = m.Quantity,
                        UnitPrice = m.UnitPrice,
                        BalanceAfter = m.BalanceAfter,
                        CreatedBy = string.IsNullOrWhiteSpace(m.CreatedBy) ? "System" : m.CreatedBy,
                        Note = m.Note
                    };
                })
                .ToList();
        }
    }
}