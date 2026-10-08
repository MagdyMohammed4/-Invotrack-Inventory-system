using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn
            };
        }

        public async Task CreateCategoryAsync(CategoryDto dto, string userName)
        {
            var category = new Category
            {
                NameAr = dto.NameAr,
                NameEn = dto.NameEn,
                CreatedBy = userName
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCategoryAsync(CategoryDto dto, string userName)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);
            if (category != null)
            {
                category.NameAr = dto.NameAr;
                category.NameEn = dto.NameEn;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteCategoryAsync(int id, string userName)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                _unitOfWork.Categories.Remove(category);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
