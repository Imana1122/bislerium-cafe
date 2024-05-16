using Application.DTO.Request.Identity;
using Application.DTO.Response.Identity;
using Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Extensions.Identity;

namespace Application.Interface.Identity
{
    public interface IAccount
    {
        //for logging in user
        Task<ServiceResponse> LoginAsync(LoginUserRequestDTO model);

        //for creating new user of any kind
        Task<ServiceResponse> CreateUserAsync(CreateUserRequestDTO model);

        //for getting all users with claims
        Task<IEnumerable<GetUserWithClaimResponseDTO>> GetUserWithClaimAsync();

        //for setting up admin account
        Task SetUpAsync();

        //for updating manager claims
        Task<ServiceResponse> UpdateUserAsync(ChangeUserClaimRequestDTO model);

        //for changing password
        Task<ServiceResponse> ChangePassword(ChangePasswordRequestDTO model);

        //for changing user basic information

        Task<ServiceResponse> ChangeSettings(ChangeSettingsRequestDTO model);

        //for deleting user account
        Task<ServiceResponse> DeleteAccountAsync(string userId);

        //for fetching user by user id

        Task<ApplicationUser> GetUserById(string userId);

        //for sending password reset url to registered email
        Task<ServiceResponse> ForgotPassword(string email, string scheme, string host, int port);

        //for resetting password
        Task<ServiceResponse> ResetPassword(ResetPasswordRequestDTO resetPassword);

    }
}
