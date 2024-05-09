using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.Identity;
using Application.Extensions.Identity;
using Application.Interface.Identity;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Application.DTO.Request.ActivityTracker;
using Application.DTO.Response.ActivityTracker;
using Mapster;
using System.Diagnostics;
using Azure.Core;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Application.Extensions;
using Application.Extensions.Email;
using Application.Service.Email;
using System.Web;


namespace Infrastructure.Repository
{
    public class Account : IAccount
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext context;
        private readonly IEmailService _emailService;
        public object Token { get; private set; }

        public Account(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context,IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this._emailService = emailService;
        }
        public async Task<ServiceResponse> CreateUserAsync(CreateUserRequestDTO model)
        {
            var user = await FindUserByEmail(model.Email);
            if (user != null)
            {
                return new ServiceResponse(false, "User already exists");
            }
            var newUser = new ApplicationUser()
            {
                UserName = model.Email,
                PasswordHash = model.Password,
                Email = model.Email,
                Name = model.Name,
            };

            var result = CheckResult(await userManager.CreateAsync(newUser, model.Password));
            if (!result.Flag)
            {
                return result;
            }
            else
            {
                return await CreateUserClaims(model);
            }

        }

        private async Task<ServiceResponse> CreateUserClaims(CreateUserRequestDTO model)
        {
            if (string.IsNullOrEmpty(model.Policy))
            {
                return new ServiceResponse(false, "No policy specified");
            }
            Claim[] userClaims = [];

            if (model.Policy.Equals(Policy.AdminPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userClaims =
                    [
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("Name", model.Name),
                    new Claim("Create","true"),
                    new Claim("Update","true"),
                    new Claim("Delete","true"),

                   ];
            }
            else if (model.Policy.Equals(Policy.ManagerPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userClaims =
                    [
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, "Manager"),
                    new Claim("Name", model.Name),
                    new Claim("Create","true"),
                    new Claim("Update","true"),
                    new Claim("Delete","false"),

                   ];
            }
            else if (model.Policy.Equals(Policy.UserPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userClaims =
                    [
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, "Blogger"),
                    new Claim("Name", model.Name),
                    new Claim("Create","false"),
                    new Claim("Update","false"),
                    new Claim("Delete","false"),

                   ];
            }
           

            var result = CheckResult(await userManager.AddClaimsAsync((await FindUserByEmail(model.Email)), userClaims));
            if (result.Flag)
            {
                return new ServiceResponse(true, "User Created");
            }
            else
            {
                return result;
            }
        }


        public async Task<IEnumerable<GetUserWithClaimResponseDTO>> GetUserWithClaimAsync()
        {
            var UserList = new List<GetUserWithClaimResponseDTO>();
            var admins = await userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Admin"));
            var managers = await userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Manager"));
            var allUsers = admins.Concat(managers).ToList(); // Materialize the concatenated result

            if (allUsers.Count == 0)
            {
                return UserList;
            }
            foreach (var user in allUsers)

            {
                var currentUser = await FindUserByEmail(user.Email);
                if (currentUser != null)
                {
                    var getCurrentUserClaims = await userManager.GetClaimsAsync(currentUser);
                    if (getCurrentUserClaims.Any())
                        UserList.Add(new GetUserWithClaimResponseDTO()
                        {
                            UserId = currentUser.Id,
                            Email = getCurrentUserClaims.FirstOrDefault(_ => _.Type == ClaimTypes.Email).Value,
                            RoleName = getCurrentUserClaims.FirstOrDefault(_ => _.Type == ClaimTypes.Role).Value,
                            Name = getCurrentUserClaims.FirstOrDefault(_ => _.Type == "Name").Value,

                            Create = Convert.ToBoolean(getCurrentUserClaims.FirstOrDefault(_ => _.Type == "Create").Value),
                            Update = Convert.ToBoolean(getCurrentUserClaims.FirstOrDefault(_ => _.Type == "Update").Value),
                            Delete = Convert.ToBoolean(getCurrentUserClaims.FirstOrDefault(_ => _.Type == "Delete").Value),

                        });
                }

               
               
            }
            return UserList;
        }

        public async Task<ServiceResponse> LoginAsync(LoginUserRequestDTO model)
        {
            var user = await FindUserByEmail(model.Email);
            var getCurrentUserClaims = await userManager.GetClaimsAsync(user);

            if (user is null) return new ServiceResponse(false, "User not found");

            var verifyPassword = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!verifyPassword.Succeeded) return new ServiceResponse(false, "Incorrect Credentials Provided");

            var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
            {
                return new ServiceResponse(false, "Unknown error occured while logging you in");

            }
            else
            {
                return new ServiceResponse(true, null);

            }


        }

        public async Task SetUpAsync() => await CreateUserAsync(new CreateUserRequestDTO
        {
            Name = "Administrator",
            Email = "admin@admin.com",
            Password = "Admin@123",
            Policy = Policy.AdminPolicy
        });

        


        public async Task<ServiceResponse> UpdateUserAsync(ChangeUserClaimRequestDTO model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is null) return new ServiceResponse(false, "User not found.");

            var oldUserClaims = await userManager.GetClaimsAsync(user);
            Claim[] newUserClaims =
                [
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, model.RoleName),
                    new Claim("Name", model.Name),
                    new Claim("Create",model.Create.ToString()),
                    new Claim("Update",model.Update.ToString()),
                    new Claim("Delete",model.Delete.ToString()),
                ];
            var result = await userManager.RemoveClaimsAsync(user, oldUserClaims);
            var response = CheckResult(result);
            if (!response.Flag)
            {
                return new ServiceResponse(false, response.Message);
            }
            var addNewClaims = await userManager.AddClaimsAsync(user, newUserClaims);
            var outcome = CheckResult(addNewClaims);
            if (outcome.Flag)
            {
                return new ServiceResponse(true, "User Updated");
            }
            else
            {
                return outcome;
            }
        }


        private async Task<ApplicationUser> FindUserByEmail(string email) => await userManager.FindByEmailAsync(email);

        private async Task<ApplicationUser> FindUserById(string id) => await userManager.FindByIdAsync(id);

        private static ServiceResponse CheckResult(IdentityResult result)
        {
            if (result.Succeeded) return new ServiceResponse(true, null);

            var errors = result.Errors.Select(_ => _.Description);
            return new ServiceResponse(false, string.Join(Environment.NewLine, errors));
        }

        public async Task<ServiceResponse> ChangePassword(ChangePasswordRequestDTO model)
        {
            var user = await userManager.FindByIdAsync(model.UserId.ToString());

            if (user == null)
            {
                // Handle user not found
                return new ServiceResponse(false,  "User not found." );
            }

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

            if (!result.Succeeded)
            {
                // Handle password change failure
                return new ServiceResponse( false, "Failed to change password." );
            }

            // Password changed successfully
            return new ServiceResponse(true,  "Password changed successfully.");
        }

        public async Task<ServiceResponse> ChangeSettings(ChangeSettingsRequestDTO model)
        {
            var user = await userManager.FindByIdAsync(model.UserId.ToString());

            if (user == null)
            {
                // Handle user not found
                return new ServiceResponse(false, "User not found.");
            }

            // Update user settings here
            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Handle user settings update failure
                return new ServiceResponse(false, "Failed to update user settings.");
            }

            // User settings updated successfully
            return new ServiceResponse(true, "User settings updated successfully.");
        }

        public async Task<ServiceResponse> DeleteAccountAsync(string userId)
        {
            var user = await FindUserById(userId.ToString());
            if (user == null)
            {
                // Handle user not found
                return new ServiceResponse(false, "User not found.");
            }
            var blogs = await context.Blogs
            .Include(b => b.Reactions) // Include related reactions
            .Include(b => b.Comments) // Include related comments
            .Include(b => b.Images) // Include related images
            .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToLower()))
            .ToListAsync();
           
            foreach (var blog in blogs)
            {
                if (blog.Reactions != null)
                {
                    foreach (var item in blog.Reactions)
                    {
                        context.BlogReactions.Remove(item);
                    }
                }

                if (blog.Images != null)
                {
                    foreach (var item in blog.Images)
                    {
                        context.BlogImages.Remove(item);
                    }
                }

                if (blog.Comments != null)
                {
                    foreach (var item in blog.Comments)
                    {
                        var comment = await context.BlogComments
                            .Include(comment => comment.Reactions) // Include related reactions
                            .FirstOrDefaultAsync(_ => _.BlogId.ToString().ToLower().Equals(item.BlogId.ToString().ToLower()) && _.UserId.ToString().ToLower().Equals(item.UserId.ToString().ToLower()));

                        if (comment != null && comment.Reactions != null)
                        {
                            foreach (var reaction in comment.Reactions)
                            {
                                context.BlogCommentReactions.Remove(reaction);
                            }
                        }

                        context.BlogComments.Remove(item);
                    }
                }

                context.Blogs.Remove(blog);
            }
            await context.SaveChangesAsync();

            var comments = await context.BlogComments
               .Include(b => b.Reactions) // Include related reactions
   
               .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToString().ToLower()))
               .ToListAsync();
            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    if (comment.Reactions != null)
                    {
                        foreach (var item in comment.Reactions)
                        {
                            context.BlogCommentReactions.Remove(item);
                        }
                    }
                    context.BlogComments.Remove(comment);

                }
            }
            await context.SaveChangesAsync();

            var reactions = await context.BlogReactions
               .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToString().ToLower()))
               .ToListAsync();

            if (reactions != null)
            {
                foreach (var reaction in reactions)
                {
                    
                    context.BlogReactions.Remove(reaction);

                }
            }
            await context.SaveChangesAsync();

            var commentReactions = await context.BlogCommentReactions
               .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToString().ToLower()) || b.CommentUserId.ToString().ToLower().Equals(user.Id.ToString().ToLower()))
               .ToListAsync();

            if (commentReactions != null)
            {
                foreach (var reaction in commentReactions)
                {

                    context.BlogCommentReactions.Remove(reaction);

                }
            }
            await context.SaveChangesAsync();

            var histories = await context.Histories
              .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToString().ToLower()) )
              .ToListAsync();

            if (histories != null)
            {
                foreach (var history in histories)
                {

                    context.Histories.Remove(history);

                }
            }
            await context.SaveChangesAsync();

            var notifications = await context.Notifications
              .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToString().ToLower()) )
              .ToListAsync();

            if (notifications != null)
            {
                foreach (var noti in notifications)
                {

                    context.Notifications.Remove(noti);

                }
            }
            await context.SaveChangesAsync();


            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                // Handle account deletion failure
                return new ServiceResponse(false, "Failed to delete user account.");
            }
            await userManager.UpdateSecurityStampAsync(user);

            // Account deleted successfully
            return new ServiceResponse(true, "User account deleted successfully.");
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            var user = await FindUserById(userId);
            if (user is null)
            {
                throw new Exception("User not found.");
            }
            else
            {
                return user;
            }


        }

        public async Task<ServiceResponse> ForgotPassword(string email,string scheme,string host,int port)
        {
            try
            {
                var user = await FindUserByEmail(email);
                if (user is null)
                {
                    return new ServiceResponse(false, "User Not Found");
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var uriBuilder = new UriBuilder();
                uriBuilder.Scheme = scheme;
                uriBuilder.Host = host; // Set your host name here
                uriBuilder.Port = port;

                uriBuilder.Path = "/Account/ResetPassword"; // Path to the ResetPassword action
                Console.WriteLine(token);

                // Add query parameters for token and email
                var query = new StringBuilder();
                query.Append($"?token={HttpUtility.UrlEncode(token)}"); // Assuming token is a variable available in your method
                query.Append($"&email={user.Email}"); // Assuming user.Email is a variable available in your method
                uriBuilder.Query = query.ToString();

                var forgotPassswordLink = uriBuilder.Uri.ToString();

                var message = new Message(new string[] { user.Email }, "Confirmation Email Link", forgotPassswordLink);
                _emailService.SendEmail(message);
                return new ServiceResponse(true, "Password Reset Link sent to your email successfully.");
            }catch (Exception ex)
            {
                return new ServiceResponse(false,ex.Message);
            }
        }

        public async Task<ServiceResponse> ResetPassword(ResetPasswordRequestDTO resetPassword)
        {
            var user = await FindUserByEmail(resetPassword.Email);
            if(user is null)
            {
                return new ServiceResponse(false, "User not found");

            }
            Console.WriteLine(resetPassword.Email); 
            Console.WriteLine($"Reset password: {resetPassword.Token}");
            var resetPassResult=await userManager.ResetPasswordAsync(user,resetPassword.Token,resetPassword.Password);
            if (!resetPassResult.Succeeded)
            {

                // Iterate over each error and collect their descriptions
                var errorDescriptions = resetPassResult.Errors.Select(error => error.Description).ToList();

                // Join the error descriptions into a single string
                var errorMessage = string.Join(", ", errorDescriptions);

                return new ServiceResponse(false, errorMessage);
            }
            return new ServiceResponse(true, "Password Reset Successfully");


        }

        
    }

}
