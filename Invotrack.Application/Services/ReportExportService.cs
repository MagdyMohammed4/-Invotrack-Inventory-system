using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class ReportExportService : IReportExportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportExportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. تصدير كافة المنتجات لملف CSV داعم للغة العربية عبر UTF-8 BOM
        public async Task<byte[]> ExportProductsToCsvAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var categories = await _unitOfWork.Categories.GetAllAsync();

            var sb = new StringBuilder();
            sb.AppendLine("SKU,اسم الصنف,التصنيف,الرصيد,سعر التكلفة,سعر البيع,إجمالي التكلفة");

            foreach (var p in products)
            {
                var catName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.NameAr ?? "-";
                var totalCost = p.TotalStock * p.CostPrice;
                sb.AppendLine($"\"{p.SKU}\",\"{p.NameAr}\",\"{catName}\",{p.TotalStock},{p.CostPrice},{p.SalePrice},{totalCost}");
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        // 2. توليد تقرير الـ PDF كصفحة HTML طباعية رسمية
        public async Task<string> GenerateValuationPdfHtmlAsync(bool isArabic)
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var categories = await _unitOfWork.Categories.GetAllAsync();

            var dir = isArabic ? "rtl" : "ltr";
            var title = isArabic ? "تقرير تقييم المخزون المالي" : "Financial Inventory Valuation Report";

            var sb = new StringBuilder();
            sb.Append($@"
            <!DOCTYPE html>
            <html lang='{(isArabic ? "ar" : "en")}' dir='{dir}'>
            <head>
                <meta charset='UTF-8'>
                <title>{title}</title>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Arial, sans-serif; margin: 30px; color: #1e293b; background: #fff; }}
                    .header {{ border-bottom: 3px solid #2563eb; padding-bottom: 15px; margin-bottom: 20px; text-align: center; }}
                    .header h2 {{ margin: 0; color: #0f172a; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ border: 1px solid #cbd5e1; padding: 10px; text-align: {(isArabic ? "right" : "left")}; }}
                    th {{ background-color: #f1f5f9; color: #0f172a; font-weight: bold; }}
                    .total-row {{ background-color: #f8fafc; font-weight: bold; }}
                    @media print {{ .no-print {{ display: none; }} }}
                </style>
            </head>
            <body>
                <div class='no-print' style='margin-bottom: 20px; text-align: left;'>
                    <button onclick='window.print()' style='background: #2563eb; color: #fff; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold;'>
                        🖨️ {(isArabic ? "طباعة / حفظ PDF" : "Print / Save PDF")}
                    </button>
                </div>

                <div class='header'>
                    <h2>مؤسسة Invotrack - {title}</h2>
                    <small style='color: #64748b;'>{(isArabic ? "تاريخ الإصدار:" : "Generated At:")} {DateTime.Now:yyyy/MM/dd HH:mm}</small>
                </div>

                <table>
                    <thead>
                        <tr>
                            <th>SKU</th>
                            <th>{(isArabic ? "اسم الصنف" : "Product Name")}</th>
                            <th>{(isArabic ? "التصنيف" : "Category")}</th>
                            <th>{(isArabic ? "الرصيد" : "Stock")}</th>
                            <th>{(isArabic ? "ت.الوحدة" : "Unit Cost")}</th>
                            <th>{(isArabic ? "إجمالي التكلفة" : "Total Cost")}</th>
                        </tr>
                    </thead>
                    <tbody>");

            decimal grandTotalCost = 0;
            foreach (var p in products)
            {
                var catName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.NameAr ?? "-";
                var totalCost = p.TotalStock * p.CostPrice;
                grandTotalCost += totalCost;

                sb.Append($@"
                        <tr>
                            <td>{p.SKU}</td>
                            <td><strong>{p.NameAr}</strong></td>
                            <td>{catName}</td>
                            <td>{p.TotalStock} {p.Unit}</td>
                            <td>{p.CostPrice:N2} ج.م</td>
                            <td><strong>{totalCost:N2} ج.م</strong></td>
                        </tr>");
            }

            sb.Append($@"
                        <tr class='total-row'>
                            <td colspan='5' style='text-align: {(isArabic ? "left" : "right")};'>{(isArabic ? "الإجمالي المالي الكلي للمخزون:" : "Grand Total Value:")}</td>
                            <td><strong style='color: #2563eb; font-size: 16px;'>{grandTotalCost:N2} ج.م</strong></td>
                        </tr>
                    </tbody>
                </table>
            </body>
            </html>");

            return sb.ToString();
        }
    }
}
