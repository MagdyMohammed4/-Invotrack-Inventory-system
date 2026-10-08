using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Enums;

namespace Invotrack.Application.DTOs
{
    public class StockReportFilterDto
    {
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public MovementType? MovementType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
       
    }
}
