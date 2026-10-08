using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Enums;
using Invotrack.Domain.Interfaces;

namespace Invotrack.Application.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PartnerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BusinessPartnerDto>> GetPartnersByTypeAsync(PartnerType type)
        {
            var partners = await _unitOfWork.BusinessPartners.GetAllAsync();
            return partners.Where(p => p.Type == type).Select(p => new BusinessPartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type,
                Phone = p.Phone,
                Email = p.Email,
                Address = p.Address
            });
        }

        public async Task<BusinessPartnerDto?> GetPartnerByIdAsync(int id)
        {
            var p = await _unitOfWork.BusinessPartners.GetByIdAsync(id);
            if (p == null) return null;

            return new BusinessPartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type,
                Phone = p.Phone,
                Email = p.Email,
                Address = p.Address
            };
        }

        public async Task CreatePartnerAsync(BusinessPartnerDto dto, string userName)
        {
            var partner = new BusinessPartner
            {
                Name = dto.Name,
                Type = dto.Type,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                CreatedBy = userName
            };

            await _unitOfWork.BusinessPartners.AddAsync(partner);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdatePartnerAsync(BusinessPartnerDto dto, string userName)
        {
            var partner = await _unitOfWork.BusinessPartners.GetByIdAsync(dto.Id);
            if (partner != null)
            {
                partner.Name = dto.Name;
                partner.Phone = dto.Phone;
                partner.Email = dto.Email;
                partner.Address = dto.Address;

                _unitOfWork.BusinessPartners.Update(partner);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeletePartnerAsync(int id, string userName)
        {
            var partner = await _unitOfWork.BusinessPartners.GetByIdAsync(id);
            if (partner != null)
            {
                _unitOfWork.BusinessPartners.Remove(partner);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
