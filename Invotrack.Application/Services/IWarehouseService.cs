using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;

namespace Invotrack.Application.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
        Task<WarehouseDto?> GetWarehouseByIdAsync(int id);
        Task CreateWarehouseAsync(WarehouseDto dto, string userName);
        Task UpdateWarehouseAsync(WarehouseDto dto, string userName);
        Task DeleteWarehouseAsync(int id, string userName);
    }
}
