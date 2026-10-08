using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class CreateAuditDto
    {
        public int ProductId { get; set; }
        public int ActualStock { get; set; }
        public string AuditorName { get; set; } = string.Empty;
        public string SignatureData { get; set; } = string.Empty;
    }
}
