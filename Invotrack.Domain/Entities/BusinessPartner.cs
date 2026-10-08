using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Enums;

namespace Invotrack.Domain.Entities
{
    public class BusinessPartner : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public PartnerType Type { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
    }
}
