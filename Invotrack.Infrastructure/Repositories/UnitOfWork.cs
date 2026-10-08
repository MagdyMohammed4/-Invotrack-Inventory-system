using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Interfaces;
using Invotrack.Infrastructure.Data;

namespace Invotrack.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InvotrackDbContext _context;
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<BusinessPartner> BusinessPartners { get; }
        public IGenericRepository<Warehouse> Warehouses { get; }
        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<StockMovement> StockMovements { get; }
        

        public UnitOfWork(InvotrackDbContext context)
        {
            _context = context;
            Categories = new GenericRepository<Category>(_context);
            BusinessPartners = new GenericRepository<BusinessPartner>(_context);
            Warehouses = new GenericRepository<Warehouse>(_context);
            Products = new GenericRepository<Product>(_context);
            StockMovements = new GenericRepository<StockMovement>(_context);
          
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}

