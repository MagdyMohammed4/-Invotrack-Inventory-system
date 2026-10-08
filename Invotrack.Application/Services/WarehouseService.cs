using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _unitOfWork.Warehouses.GetAllAsync();
            return warehouses.Select(w => new WarehouseDto
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location
            });
        }

        public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse == null) return null;

            return new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location
            };
        }

        public async Task CreateWarehouseAsync(WarehouseDto dto, string userName)
        {
            var warehouse = new Warehouse
            {
                Name = dto.Name,
                Location = dto.Location,
                CreatedBy = userName
            };

            await _unitOfWork.Warehouses.AddAsync(warehouse);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateWarehouseAsync(WarehouseDto dto, string userName)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(dto.Id);
            if (warehouse != null)
            {
                warehouse.Name = dto.Name;
                warehouse.Location = dto.Location;

                _unitOfWork.Warehouses.Update(warehouse);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteWarehouseAsync(int id, string userName)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse != null)
            {
                _unitOfWork.Warehouses.Remove(warehouse);
                await _unitOfWork.CompleteAsync();
            }
        }

    }
}
