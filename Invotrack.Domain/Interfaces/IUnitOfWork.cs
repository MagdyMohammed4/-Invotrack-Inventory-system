using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Entities;

namespace Invotrack.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<BusinessPartner> BusinessPartners { get; }
        IGenericRepository<Warehouse> Warehouses { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<StockMovement> StockMovements { get; }
        Task<int> CompleteAsync();
    }
}
