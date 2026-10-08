using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;

namespace Invotrack.Application.Services
{
    public interface IStockReportService
    {
        Task<IEnumerable<StockReportItemDto>> GetMovementsReportAsync(StockReportFilterDto filter);
    }
}
