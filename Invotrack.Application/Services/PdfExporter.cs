using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;

namespace Invotrack.Application.Services
{
    public class PdfExporter : IPdfExporter
    {
        public string GenerateProductsReportHtml(IEnumerable<ProductDto> products, bool isArabic)
        {
            var dir = isArabic ? "rtl" : "ltr";
            var title = isArabic ? "تقرير المخزون والمنتجات" : "Inventory Products Report";

            var sb = new StringBuilder();
            sb.Append($@"
            <!DOCTYPE html>
            <html lang='{(isArabic ? "ar" : "en")}' dir='{dir}'>
            <head>
                <meta charset='UTF-8'>
                <title>{title}</title>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 20px; background: #fff; color: #1e293b; }}
                    .header {{ border-bottom: 3px solid #2563eb; padding-bottom: 10px; margin-bottom: 20px; text-align: center; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 15px; }}
                    th, td {{ border: 1px solid #cbd5e1; padding: 10px; text-align: {(isArabic ? "right" : "left")}; }}
                    th {{ background-color: #f1f5f9; font-weight: bold; }}
                    .low-stock {{ color: #dc2626; font-weight: bold; }}
                    @media print {{ .no-print {{ display: none; }} }}
                </style>
            </head>
            <body>
                <div class='no-print' style='margin-bottom: 15px;'>
                    <button onclick='window.print()' style='padding: 8px 16px; background: #2563eb; color: white; border: none; border-radius: 4px; cursor: pointer;'>
                        {(isArabic ? "طباعة / حفظ PDF" : "Print / Save PDF")}
                    </button>
                </div>

                <div class='header'>
                    <h2>Invotrack System - {title}</h2>
                    <small>{DateTime.Now:yyyy-MM-dd HH:mm}</small>
                </div>

                <table>
                    <thead>
                        <tr>
                            <th>SKU</th>
                            <th>{(isArabic ? "اسم الصنف" : "Product Name")}</th>
                            <th>{(isArabic ? "التصنيف" : "Category")}</th>
                            <th>{(isArabic ? "سعر الشراء" : "Cost")}</th>
                            <th>{(isArabic ? "سعر البيع" : "Price")}</th>
                            <th>{(isArabic ? "الرصيد" : "Stock")}</th>
                        </tr>
                    </thead>
                    <tbody>");

            foreach (var item in products)
            {
                var name = isArabic ? item.NameAr : item.NameEn;
                sb.Append($@"
                        <tr>
                            <td>{item.SKU}</td>
                            <td><strong>{name}</strong></td>
                            <td>{item.CategoryName}</td>
                            <td>{item.CostPrice:N2}</td>
                            <td>{item.SalePrice:N2}</td>
                            <td class='{(item.IsLowStock ? "low-stock" : "")}'>{item.TotalStock} {item.Unit}</td>
                        </tr>");
            }

            sb.Append(@"
                    </tbody>
                </table>
            </body>
            </html>");

            return sb.ToString();
        }

        public string GenerateTransactionHtml(TransactionReceiptDto receipt)
        {
            var sb = new StringBuilder();
            sb.Append($@"
            <!DOCTYPE html>
            <html lang='ar' dir='rtl'>
            <head>
                <meta charset='UTF-8'>
                <title>{receipt.TypeName} - {receipt.DocumentNo}</title>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Arial, sans-serif; margin: 30px; background: #fff; color: #1e293b; }}
                    .receipt-box {{ border: 2px solid #2563eb; padding: 25px; border-radius: 10px; max-width: 700px; margin: 0 auto; }}
                    .header {{ border-bottom: 2px dashed #cbd5e1; padding-bottom: 15px; margin-bottom: 20px; text-align: center; }}
                    .header h2 {{ margin: 0; color: #2563eb; }}
                    .info-table {{ width: 100%; margin-bottom: 20px; border-collapse: collapse; }}
                    .info-table td {{ padding: 8px; vertical-align: top; }}
                    .items-table {{ width: 100%; border-collapse: collapse; margin-top: 15px; }}
                    .items-table th, .items-table td {{ border: 1px solid #cbd5e1; padding: 10px; text-align: center; }}
                    .items-table th {{ background-color: #f1f5f9; }}
                    .footer {{ margin-top: 30px; text-align: center; font-size: 13px; color: #64748b; border-top: 1px solid #e2e8f0; padding-top: 15px; }}
                    @media print {{ .no-print {{ display: none; }} }}
                </style>
            </head>
            <body>
                <div class='no-print' style='text-align: center; margin-bottom: 20px;'>
                    <button onclick='window.print()' style='padding: 10px 25px; background: #2563eb; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: bold;'>
                        🖨️ طباعة الإذن / حفظ PDF
                    </button>
                </div>

                <div class='receipt-box'>
                    <div class='header'>
                        <h2>منظومة Invotrack للمخازن</h2>
                        <h3 style='margin: 5px 0;'>{receipt.TypeName}</h3>
                        <small>رقم المستند: <strong>{receipt.DocumentNo}</strong> | التاريخ: {receipt.Date:yyyy/MM/dd HH:mm}</small>
                    </div>

                    <table class='info-table'>
                        <tr>
                            <td><strong>المخزن المعني:</strong> {receipt.WarehouseName}</td>
                            <td><strong>المسؤول:</strong> {receipt.HandledBy}</td>
                        </tr>
                        <tr>
                            <td colspan='2'><strong>ملاحظات / البيان:</strong> {receipt.Note}</td>
                        </tr>
                    </table>

                    <table class='items-table'>
                        <thead>
                            <tr>
                                <th>كود الصنف (SKU)</th>
                                <th>اسم الصنف</th>
                                <th>الكمية</th>
                                <th>سعر التكلفة</th>
                                <th>الإجمالي</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>{receipt.SKU}</td>
                                <td><strong>{receipt.ProductName}</strong></td>
                                <td>{receipt.Quantity} {receipt.Unit}</td>
                                <td>{receipt.UnitCost:N2} ج.م</td>
                                <td><strong>{receipt.TotalCost:N2} ج.م</strong></td>
                            </tr>
                        </tbody>
                    </table>

                    <div class='footer'>
                        <p>تم إستخراج هذا المستند إلكترونياً من نظام Invotrack</p>
                    </div>
                </div>
            </body>
            </html>");

            return sb.ToString();
        }
    }
}

