using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class WarehouseDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المخزن مطلوب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "الموقع مطلوب")]
        public string Location { get; set; } = string.Empty;
    }
}
