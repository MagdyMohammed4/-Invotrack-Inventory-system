using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class ProductLedgerDto
    {
        public DateTime Date { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string MovementType { get; set; } = string.Empty;
        public string SourceWarehouse { get; set; } = string.Empty;
        public string DestinationWarehouse { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int BalanceAfter { get; set; }
        public string ExecutedBy { get; set; } = string.Empty;
    }
}
