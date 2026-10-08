using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Enums;

namespace Invotrack.Application.Services
{
    public interface IPartnerService
    {
        Task<IEnumerable<BusinessPartnerDto>> GetPartnersByTypeAsync(PartnerType type);
        Task<BusinessPartnerDto?> GetPartnerByIdAsync(int id);
        Task CreatePartnerAsync(BusinessPartnerDto dto, string userName);
        Task UpdatePartnerAsync(BusinessPartnerDto dto, string userName);
        Task DeletePartnerAsync(int id, string userName);
    }
}
