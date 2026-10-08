using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.Services
{
    public interface IReportExportService
    {
        Task<byte[]> ExportProductsToCsvAsync();
        Task<string> GenerateValuationPdfHtmlAsync(bool isArabic);
    }
}
