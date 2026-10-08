using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Domain.Enums;

namespace Invotrack.Application.DTOs
{
    public class BusinessPartnerDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; } = string.Empty;
        public PartnerType Type { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
    }
}
