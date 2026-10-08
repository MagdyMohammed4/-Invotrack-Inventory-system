using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1️⃣ عرض المنتجات بنظام الصفحات مع البحث الذكي
        public async Task<PagedResultDto<ProductDto>> GetProductsPagedAsync(string? search, int pageIndex, int pageSize)
        {
            var (items, totalCount) = await _unitOfWork.Products.GetPagedAsync(
                pageIndex, pageSize,
                p => string.IsNullOrEmpty(search) ||
                     p.NameAr.Contains(search) ||
                     (!string.IsNullOrEmpty(p.NameEn) && p.NameEn.Contains(search)) ||
                     p.SKU.Contains(search),
                p => p.Category!);

            var dtos = items.Select(p => new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                NameAr = p.NameAr,
                NameEn = p.NameEn ?? string.Empty,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.NameAr ?? "-",
                Unit = p.Unit,
                CostPrice = p.CostPrice,
                SalePrice = p.SalePrice,
                TotalStock = p.TotalStock,
                MinReorderQuantity = p.MinReorderQuantity
            });

            return new PagedResultDto<ProductDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        // 2️⃣ جلب تفاصيل منتج محدد بواسطة الـ Id
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var p = await _unitOfWork.Products.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                NameAr = p.NameAr,
                NameEn = p.NameEn,
                CategoryId = p.CategoryId,
                Unit = p.Unit,
                CostPrice = p.CostPrice,
                SalePrice = p.SalePrice,
                TotalStock = p.TotalStock,
                MinReorderQuantity = p.MinReorderQuantity
            };
        }

        // 3️⃣ إضافة منتج جديد (توليد الـ SKU تلقائياً في حال تركه فارغاً)
        public async Task CreateProductAsync(ProductDto dto, string userName)
        {
            var allProducts = await _unitOfWork.Products.GetAllAsync();

            // أ) التوليد التلقائي المباشر للـ SKU عند الترُك فارغاً
            string finalSku;
            if (string.IsNullOrWhiteSpace(dto.SKU))
            {
                finalSku = await GenerateUniqueSkuAsync();
                dto.SKU = finalSku; // تحديث الـ DTO بالقيمة الجديدة المولدة
            }
            else
            {
                finalSku = dto.SKU.Trim().ToUpper();
                // التحقق من عدم تكرار الـ SKU المدخل يدوياً
                if (allProducts.Any(p => p.SKU.Equals(finalSku, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"رمز SKU '{finalSku}' مستخدم بالفعل لمنتج آخر.");
                }
            }

            // ب) التحقق من عدم تكرار الاسم العربي
            if (allProducts.Any(p => p.NameAr.Trim().Equals(dto.NameAr.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"اسم المنتج العربي '{dto.NameAr}' مكرر وموجود بالفعل.");
            }

            // ج) التحقق من عدم تكرار الاسم الإنجليزي (إن وجد)
            if (!string.IsNullOrWhiteSpace(dto.NameEn) &&
                allProducts.Any(p => !string.IsNullOrWhiteSpace(p.NameEn) && p.NameEn.Trim().Equals(dto.NameEn.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"اسم المنتج بالإنجليزي '{dto.NameEn}' مكرر وموجود بالفعل.");
            }

            var product = new Product
            {
                SKU = finalSku,
                NameAr = dto.NameAr.Trim(),
                NameEn = dto.NameEn?.Trim(),
                CategoryId = dto.CategoryId,
                Unit = dto.Unit,
                CostPrice = dto.CostPrice,
                SalePrice = dto.SalePrice,
                TotalStock = dto.TotalStock,
                MinReorderQuantity = dto.MinReorderQuantity,
                CreatedBy = userName,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        // 4️⃣ تعديل منتج قائم مع حفظ الـ SKU الحالي إذا تُرك فارغاً
        public async Task UpdateProductAsync(ProductDto dto, string userName)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
            if (product == null)
            {
                throw new KeyNotFoundException("المنتج المطلوب غير موجود بالبيانات.");
            }

            var allProducts = await _unitOfWork.Products.GetAllAsync();

            string finalSku = string.IsNullOrWhiteSpace(dto.SKU) ? product.SKU : dto.SKU.Trim().ToUpper();

            // أ) التحقق من عدم تكرار الـ SKU مع منتج آخر غير المنتج الحالي
            if (allProducts.Any(p => p.Id != dto.Id && p.SKU.Equals(finalSku, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"رمز SKU '{finalSku}' مخصص لمنتج آخر بالفعل.");
            }

            // ب) التحقق من عدم تكرار الاسم العربي مع منتج آخر
            if (allProducts.Any(p => p.Id != dto.Id && p.NameAr.Trim().Equals(dto.NameAr.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"اسم المنتج العربي '{dto.NameAr}' مستخدم لمنتج آخر.");
            }

            // ج) التحقق من عدم تكرار الاسم الإنجليزي مع منتج آخر
            if (!string.IsNullOrWhiteSpace(dto.NameEn) &&
                allProducts.Any(p => p.Id != dto.Id && !string.IsNullOrWhiteSpace(p.NameEn) && p.NameEn.Trim().Equals(dto.NameEn.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"اسم المنتج الإنجليزي '{dto.NameEn}' مستخدم لمنتج آخر.");
            }

            product.SKU = finalSku;
            product.NameAr = dto.NameAr.Trim();
            product.NameEn = dto.NameEn?.Trim();
            product.CategoryId = dto.CategoryId;
            product.Unit = dto.Unit;
            product.CostPrice = dto.CostPrice;
            product.SalePrice = dto.SalePrice;
            product.MinReorderQuantity = dto.MinReorderQuantity;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
        }

        // 5️⃣ حذف منتج
        public async Task DeleteProductAsync(int id, string userName)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.Products.Remove(product);
                await _unitOfWork.CompleteAsync();
            }
        }

        // 🟢 دالة توليد الـ SKU التلقائي والفريد
        private async Task<string> GenerateUniqueSkuAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            int counter = products.Count() + 1;
            string newSku;

            do
            {
                newSku = $"PRD-{counter:D3}";
                counter++;
            }
            while (products.Any(p => p.SKU.Equals(newSku, StringComparison.OrdinalIgnoreCase)));

            return newSku;
        }
    }
}