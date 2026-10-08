using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invotrack.Application.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "الاسم العربي مطلوب")]
        public string NameAr { get; set; } = string.Empty;
        [Required(ErrorMessage = "English Name is required")]
        public string NameEn { get; set; } = string.Empty;
    }
}
