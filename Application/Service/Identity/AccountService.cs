using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.Identity;
using Application.Extensions.Identity;
using Application.Interface.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Identity
{
    public class AccountService(IAccount account) : IAccountService
    {
        public Task<ServiceResponse> ChangePassword(ChangePasswordRequestDTO model)
        {
            return account.ChangePassword(model);
        }

        public Task<ServiceResponse> ChangeSettings(ChangeSettingsRequestDTO model)
        {
            return account.ChangeSettings(model);
        }

        public Task<ServiceResponse> CreateUserAsync(CreateUserRequestDTO model)
        {
            return account.CreateUserAsync(model);
        }

        public Task<ServiceResponse> DeleteAccountAsync(string userId)
        {
            return account.DeleteAccountAsync(userId);
        }

        public Task<ServiceResponse> ForgotPassword(string email, string scheme, string host, int port)
        {
            return account.ForgotPassword(email, scheme,host,port);
        }

        public Task<ApplicationUser> GetUserById(string userId)
        {
            return account.GetUserById(userId);
        }

        public Task<IEnumerable<GetUserWithClaimResponseDTO>> GetUserWithClaimAsync()
        {
            return account.GetUserWithClaimAsync();
        }

        public Task<ServiceResponse> LoginAsync(LoginUserRequestDTO model)
        {
            return account.LoginAsync(model);
        }

        public Task<ServiceResponse> ResetPassword(ResetPasswordRequestDTO resetPassword)
        {
            return account.ResetPassword(resetPassword);
        }

        public Task SetUpAsync()
        {
            return account.SetUpAsync();
        }

        public Task<ServiceResponse> UpdateUserAsync(ChangeUserClaimRequestDTO model)
        {
            return account.UpdateUserAsync(model);
        }

        
           
    }
}
