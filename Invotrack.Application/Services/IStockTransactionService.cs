using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;

namespace Invotrack.Application.Services
{
    public interface IStockTransactionService
    {
        Task ProcessStockInAsync(StockInDto dto, string userName);
        Task ProcessStockOutAsync(StockOutDto dto, string userName);
    }
}
