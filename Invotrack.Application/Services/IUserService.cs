using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Invotrack.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserListDto>> GetAllUsersAsync();
        Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();
        Task<IdentityResult> CreateUserAsync(CreateUserDto dto);
        Task<bool> ToggleUserStatusAsync(string userId);
    }
}
