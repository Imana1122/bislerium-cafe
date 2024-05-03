using Application.DTO.Request.ActivityTracker;
using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.ActivityTracker;
using Application.DTO.Response.Identity;
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
        public Task<ServiceResponse> CreateUserAsync(CreateUserRequestDTO model)
        {
            return account.CreateUserAsync(model);
        }

        public Task<IEnumerable<ActivityTrackerResponseDTO>> GetActivitiesAsync()
        {
            return account.GetActivitiesAsync();
        }

        public Task<IEnumerable<GetUserWithClaimResponseDTO>> GetUserWithClaimAsync()
        {
            return account.GetUserWithClaimAsync();
        }

        public Task<ServiceResponse> LoginAsync(LoginUserRequestDTO model)
        {
            return account.LoginAsync(model);
        }

        public Task SaveActivityAsync(ActivityTrackerRequestDTO model)
        {
            return account.SaveActivityAsync(model);
        }

        public Task SetUpAsync()
        {
            return account.SetUpAsync();
        }

        public Task<ServiceResponse> UpdateUserAsync(ChangeUserClaimRequestDTO model)
        {
            return account.UpdateUserAsync(model);
        }

        public async Task<IEnumerable<IGrouping<DateTime, ActivityTrackerResponseDTO>>> GroupActivities()
        {
            var data = (await GetActivitiesAsync()).GroupBy(e => e.Date).AsEnumerable();
            return data;
        }
           
    }
}
