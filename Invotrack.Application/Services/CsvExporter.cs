using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.Services
{
    public class CsvExporter : ICsvExporter
    {
        public byte[] ExportToCsv<T>(IEnumerable<T> data, string[] headers, Func<T, string[]> rowSelector)
        {
            var sb = new StringBuilder();

            // إضافة العناوين الرئيسية
            sb.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

            // إضافة الصفوف
            foreach (var item in data)
            {
                var row = rowSelector(item);
                sb.AppendLine(string.Join(",", row.Select(r => $"\"{r?.Replace("\"", "\"\"")}\"")));
            }

            // تضمين UTF-8 Encoding BOM لإغلاق مشاكل الترميز في Excel
            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }
    }
}

