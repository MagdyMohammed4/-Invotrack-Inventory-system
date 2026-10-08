using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class StockMovementDto
    {
        public int ProductId { get; set; }
        public int SourceWarehouseId { get; set; }
        public int? DestinationWarehouseId { get; set; }
        public int? BusinessPartnerId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من 0")]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
