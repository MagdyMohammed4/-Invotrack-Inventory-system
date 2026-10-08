using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Enums;

namespace Invotrack.Domain.Entities
{
    public class StockMovement : BaseEntity
    {
        public string DocumentNo { get; set; } = string.Empty;
        public MovementType Type { get; set; }

        // الصنف
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // المخزن المصدر (المستند/المصروف منه)
        public int SourceWarehouseId { get; set; }
        public Warehouse? SourceWarehouse { get; set; }

        // المخزن الوجهة (خاص بحركات النقل Transfer)
        public int? DestinationWarehouseId { get; set; }
        public Warehouse? DestinationWarehouse { get; set; }

        // المورد أو الموزع المرتبط بالحركة
        public int? BusinessPartnerId { get; set; }
        public BusinessPartner? BusinessPartner { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int BalanceAfter { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
