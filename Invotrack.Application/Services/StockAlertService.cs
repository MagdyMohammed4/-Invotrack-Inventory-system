using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class StockAlertService : IStockAlertService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockAlertService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // جلب المنتجات التي وصل رصيدها للحد الأدنى أو أقل
        public async Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var categories = await _unitOfWork.Categories.GetAllAsync();

            return products
                .Where(p => p.TotalStock <= p.MinReorderQuantity)
                .Select(p => new LowStockProductDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    NameAr = p.NameAr,
                    NameEn = p.NameEn,
                    CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.NameAr ?? "-",
                    Unit = p.Unit,
                    TotalStock = p.TotalStock,
                    MinReorderQuantity = p.MinReorderQuantity
                });
        }

        // إرجاع عدد المنتجات المنخفضة لاستخدامها في إشعارات النوافذ الهيدر
        public async Task<int> GetLowStockCountAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return products.Count(p => p.TotalStock <= p.MinReorderQuantity);
        }
    }
}
