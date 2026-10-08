using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;

namespace Invotrack.Application.Services
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetProductsPagedAsync(string? search, int pageIndex, int pageSize);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task CreateProductAsync(ProductDto dto, string userName);
        Task UpdateProductAsync(ProductDto dto, string userName);
        Task DeleteProductAsync(int id, string userName);
    }
}
