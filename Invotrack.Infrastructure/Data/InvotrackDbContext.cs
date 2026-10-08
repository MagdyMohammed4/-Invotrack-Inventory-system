using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Invotrack.Infrastructure.Data
{
    public class InvotrackDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public InvotrackDbContext(DbContextOptions<InvotrackDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<BusinessPartner> BusinessPartners { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<StockMovement>().Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
        }
    }
}
