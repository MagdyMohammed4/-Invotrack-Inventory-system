using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;

namespace Invotrack.Application.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CategoryDto dto, string userName);
        Task UpdateCategoryAsync(CategoryDto dto, string userName);
        Task DeleteCategoryAsync(int id, string userName);
    }
}
