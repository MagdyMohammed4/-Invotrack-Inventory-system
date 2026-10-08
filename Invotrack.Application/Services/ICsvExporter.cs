using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.Services
{
    public interface ICsvExporter
    {
        byte[] ExportToCsv<T>(IEnumerable<T> data, string[] headers, Func<T, string[]> rowSelector);
    }
}
