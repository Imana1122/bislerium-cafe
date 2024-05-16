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
        //for changing password
        public Task<ServiceResponse> ChangePassword(ChangePasswordRequestDTO model)
        {
            return account.ChangePassword(model);
        }

        //for changing basic user information
        public Task<ServiceResponse> ChangeSettings(ChangeSettingsRequestDTO model)
        {
            return account.ChangeSettings(model);
        }

        
        //for creating any new user
        public Task<ServiceResponse> CreateUserAsync(CreateUserRequestDTO model)
        {
            return account.CreateUserAsync(model);
        }

        //for deleting user account
        public Task<ServiceResponse> DeleteAccountAsync(string userId)
        {
            return account.DeleteAccountAsync(userId);
        }

        //for sending password reset link to registered user
        public Task<ServiceResponse> ForgotPassword(string email, string scheme, string host, int port)
        {
            return account.ForgotPassword(email, scheme,host,port);
        }

        //for fetching user by id

        public Task<ApplicationUser> GetUserById(string userId)
        {
            return account.GetUserById(userId);
        }

        //for fetching user with claims

        public Task<IEnumerable<GetUserWithClaimResponseDTO>> GetUserWithClaimAsync()
        {
            return account.GetUserWithClaimAsync();
        }

        //for logging user

        public Task<ServiceResponse> LoginAsync(LoginUserRequestDTO model)
        {
            return account.LoginAsync(model);
        }

        //for resetting password

        public Task<ServiceResponse> ResetPassword(ResetPasswordRequestDTO resetPassword)
        {
            return account.ResetPassword(resetPassword);
        }

        //for setting up admin
        public Task SetUpAsync()
        {
            return account.SetUpAsync();
        }

        //for updating manager claims

        public Task<ServiceResponse> UpdateUserAsync(ChangeUserClaimRequestDTO model)
        {
            return account.UpdateUserAsync(model);
        }

        
           
    }
}
