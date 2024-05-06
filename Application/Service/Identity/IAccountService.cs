using Application.DTO.Request.Identity;
using Application.DTO.Response.Identity;
using Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Request.ActivityTracker;
using Application.DTO.Response.ActivityTracker;
using Application.Extensions.Identity;

namespace Application.Service.Identity
{
    public interface IAccountService
    {
        Task<ServiceResponse> LoginAsync(LoginUserRequestDTO model);
        Task<ServiceResponse> CreateUserAsync(CreateUserRequestDTO model);
        Task<IEnumerable<GetUserWithClaimResponseDTO>> GetUserWithClaimAsync();
        Task SetUpAsync();
        Task<ServiceResponse> UpdateUserAsync(ChangeUserClaimRequestDTO model);

        Task<ServiceResponse> ChangePassword(ChangePasswordRequestDTO model);

        Task<ServiceResponse> ChangeSettings(ChangeSettingsRequestDTO model);
        Task<ServiceResponse> DeleteAccountAsync(string userId);
        Task<ApplicationUser> GetUserById(string userId);
        Task<ServiceResponse> ForgotPassword(string email, string scheme, string host, int port);
        Task<ServiceResponse> ResetPassword(ResetPasswordRequestDTO resetPassword);


    }
}
