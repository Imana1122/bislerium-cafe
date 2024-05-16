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
        // UserManager to manage user-related operations
        private readonly UserManager<ApplicationUser> userManager;
        // SignInManager for user sign-in functionalities
        private readonly SignInManager<ApplicationUser> signInManager;
        // Database context for interacting with the database
        private readonly AppDbContext context;
        // Service for sending emails
        private readonly IEmailService _emailService;

        // Constructor to initialize dependencies
        public Account(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context, IEmailService emailService)
        {
            // Initialize UserManager
            this.userManager = userManager;
            // Initialize SignInManager
            this.signInManager = signInManager;
            // Initialize database context
            this.context = context;
            // Initialize email service
            this._emailService = emailService;
        }

        /// <summary>
        /// Method to create a new user asynchronously.
        /// </summary>
        /// <param name="model">The user creation request DTO containing user information.</param>
        /// <returns>A service response indicating the result of the user creation operation.</returns>
        public async Task<ServiceResponse> CreateUserAsync(CreateUserRequestDTO model)
        {
            // Check if a user with the provided email already exists
            var user = await FindUserByEmail(model.Email);

            // If a user with the provided email already exists, return an error response
            if (user != null)
            {
                return new ServiceResponse(false, "User already exists");
            }

            // Create a new ApplicationUser object with the provided user information
            var newUser = new ApplicationUser()
            {
                UserName = model.Email,
                PasswordHash = model.Password,
                Email = model.Email,
                Name = model.Name,
            };

            // Create the user using UserManager and the provided password
            var result = CheckResult(await userManager.CreateAsync(newUser, model.Password));

            // If user creation was not successful, return the result with error message
            if (!result.Flag)
            {
                return result;
            }
            else
            {
                // If user creation was successful, create user claims
                return await CreateUserClaims(model);
            }
        }


        /// <summary>
        /// Creates and assigns claims to a new user based on the specified user creation policy.
        /// </summary>
        /// <param name="model">The user creation request DTO containing user information and policy.</param>
        /// <returns>A service response indicating the result of the user claims creation operation.</returns>
        private async Task<ServiceResponse> CreateUserClaims(CreateUserRequestDTO model)
        {
            // Check if a policy is specified
            if (string.IsNullOrEmpty(model.Policy))
            {
                // If no policy is specified, return an error response
                return new ServiceResponse(false, "No policy specified");
            }

            Claim[] userClaims = null;

            // Create claims based on the specified policy
            if (model.Policy.Equals(Policy.AdminPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userClaims = new[]
                {
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("Name", model.Name),
            new Claim("Create", "true"),
            new Claim("Update", "true"),
            new Claim("Delete", "true"),
        };
            }
            else if (model.Policy.Equals(Policy.ManagerPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userClaims = new[]
                {
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(ClaimTypes.Role, "Manager"),
            new Claim("Name", model.Name),
            new Claim("Create", "true"),
            new Claim("Update", "true"),
            new Claim("Delete", "false"),
        };
            }
            else if (model.Policy.Equals(Policy.UserPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userClaims = new[]
                {
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(ClaimTypes.Role, "Blogger"),
            new Claim("Name", model.Name),
            new Claim("Create", "false"),
            new Claim("Update", "false"),
            new Claim("Delete", "false"),
        };
            }

            // Add claims to the user
            var result = CheckResult(await userManager.AddClaimsAsync(await FindUserByEmail(model.Email), userClaims));

            // Return a response indicating the result of the user claims creation operation
            if (result.Flag)
            {
                return new ServiceResponse(true, "User Created");
            }
            else
            {
                return result;
            }
        }



        /// <summary>
        /// Retrieves users along with their associated claims asynchronously.
        /// </summary>
        /// <returns>A collection of DTOs representing users with their claims.</returns>
        public async Task<IEnumerable<GetUserWithClaimResponseDTO>> GetUserWithClaimAsync()
        {
            // Initialize a list to hold the user DTOs
            var UserList = new List<GetUserWithClaimResponseDTO>();

            // Retrieve users with the "Admin" role
            var admins = await userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Admin"));

            // Retrieve users with the "Manager" role
            var managers = await userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Manager"));

            // Combine the lists of admin and manager users
            var allUsers = admins.Concat(managers).ToList(); // Materialize the concatenated result

            // If no users found, return an empty list
            if (allUsers.Count == 0)
            {
                return UserList;
            }

            // Iterate through each user
            foreach (var user in allUsers)
            {
                // Find the current user by email
                var currentUser = await FindUserByEmail(user.Email);

                // If the user exists
                if (currentUser != null)
                {
                    // Retrieve the claims associated with the current user
                    var getCurrentUserClaims = await userManager.GetClaimsAsync(currentUser);

                    // If the user has any claims
                    if (getCurrentUserClaims.Any())
                    {
                        // Create a DTO representing the user with claims and add it to the list
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
            }

            // Return the list of user DTOs
            return UserList;
        }



        /// <summary>
        /// Asynchronously attempts to log in a user based on the provided credentials.
        /// </summary>
        /// <param name="model">The login user request DTO containing user credentials.</param>
        /// <returns>A service response indicating the result of the login operation.</returns>
        public async Task<ServiceResponse> LoginAsync(LoginUserRequestDTO model)
        {
            // Find the user by email
            var user = await FindUserByEmail(model.Email);

            // Get the claims associated with the user
            var getCurrentUserClaims = await userManager.GetClaimsAsync(user);

            // If user is not found, return an error response
            if (user is null)
            {
                return new ServiceResponse(false, "User not found");
            }

            // Verify the provided password
            var verifyPassword = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            // If password verification fails, return an error response
            if (!verifyPassword.Succeeded)
            {
                return new ServiceResponse(false, "Incorrect credentials provided");
            }

            // Attempt to sign in the user
            var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

            // If sign-in operation fails, return an error response
            if (!result.Succeeded)
            {
                return new ServiceResponse(false, "Unknown error occurred while logging you in");
            }
            else
            {
                // If sign-in operation succeeds, return a success response
                return new ServiceResponse(true, null);
            }
        }



        /// <summary>
        /// Asynchronously sets up the initial configuration by creating an administrator user.
        /// </summary>
        /// <remarks>
        /// This method is typically used during application initialization to create an initial administrator account.
        /// </remarks>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SetUpAsync() => await CreateUserAsync(new CreateUserRequestDTO
        {
            Name = "Administrator", // Set the name of the administrator user
            Email = "admin@admin.com", // Set the email of the administrator user
            Password = "Admin@123", // Set the password of the administrator user
            Policy = "AdminPolicy" // Set the policy for the administrator user
        });





        /// <summary>
        /// Asynchronously updates user claims based on the provided request DTO.
        /// </summary>
        /// <param name="model">The change user claim request DTO containing updated user claim information.</param>
        /// <returns>A service response indicating the result of the user claim update operation.</returns>
        public async Task<ServiceResponse> UpdateUserAsync(ChangeUserClaimRequestDTO model)
        {
            // Find the user by ID
            var user = await userManager.FindByIdAsync(model.UserId);

            // If user is not found, return an error response
            if (user is null)
            {
                return new ServiceResponse(false, "User not found.");
            }

            // Define the new user claims based on the provided model
            Claim[] newUserClaims =
            {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, model.RoleName),
        new Claim("Name", model.Name),
        new Claim("Create", model.Create.ToString()),
        new Claim("Update", model.Update.ToString()),
        new Claim("Delete", model.Delete.ToString()),
    };

            // Remove old user claims
            var result = await userManager.RemoveClaimsAsync(user, await userManager.GetClaimsAsync(user));

            // Check the result of removing old claims
            var response = CheckResult(result);
            if (!response.Flag)
            {
                // If removing old claims failed, return an error response
                return new ServiceResponse(false, response.Message);
            }

            // Add new user claims
            var addNewClaims = await userManager.AddClaimsAsync(user, newUserClaims);

            // Check the result of adding new claims
            var outcome = CheckResult(addNewClaims);

            await userManager.UpdateSecurityStampAsync(user);
            if (outcome.Flag)
            {
                // If adding new claims succeeded, return a success response
                return new ServiceResponse(true, "User Updated");
            }
            else
            {
                // If adding new claims failed, return an error response
                return outcome;
            }
        }



        /// <summary>
        /// Asynchronously finds a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to find.</param>
        /// <returns>The user with the specified email address, if found; otherwise, null.</returns>
        private async Task<ApplicationUser> FindUserByEmail(string email) => await userManager.FindByEmailAsync(email);


        /// <summary>
        /// Asynchronously finds a user by their user ID.
        /// </summary>
        /// <param name="id">The ID of the user to find.</param>
        /// <returns>The user with the specified ID, if found; otherwise, null.</returns>
        private async Task<ApplicationUser> FindUserById(string id) => await userManager.FindByIdAsync(id);


        /// <summary>
        /// Checks the result of an identity operation and returns a service response accordingly.
        /// </summary>
        /// <param name="result">The result of the identity operation.</param>
        /// <returns>A service response indicating the outcome of the operation.</returns>
        private static ServiceResponse CheckResult(IdentityResult result)
        {
            // If the operation succeeded, return a success response
            if (result.Succeeded)
            {
                return new ServiceResponse(true, null);
            }

            // If the operation failed, gather error descriptions and return an error response
            var errors = result.Errors.Select(_ => _.Description);
            return new ServiceResponse(false, string.Join(Environment.NewLine, errors));
        }


        /// <summary>
        /// Asynchronously changes the password for a user.
        /// </summary>
        /// <param name="model">The change password request DTO containing user ID, old password, and new password.</param>
        /// <returns>A service response indicating the result of the password change operation.</returns>
        public async Task<ServiceResponse> ChangePassword(ChangePasswordRequestDTO model)
        {
            // Find the user by ID
            var user = await userManager.FindByIdAsync(model.UserId.ToString());

            // If user is not found, return an error response
            if (user == null)
            {
                return new ServiceResponse(false, "User not found.");
            }

            // Attempt to change the password
            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

            // If password change operation failed, return an error response
            if (!result.Succeeded)
            {
                return new ServiceResponse(false, "Failed to change password.");
            }

            // Password changed successfully, return a success response
            return new ServiceResponse(true, "Password changed successfully.");
        }


        /// <summary>
        /// Asynchronously changes the settings for a user.
        /// </summary>
        /// <param name="model">The change settings request DTO containing user ID, name, and email.</param>
        /// <returns>A service response indicating the result of the settings change operation.</returns>
        public async Task<ServiceResponse> ChangeSettings(ChangeSettingsRequestDTO model)
        {
            // Find the user by ID
            var user = await userManager.FindByIdAsync(model.UserId.ToString());

            // If user is not found, return an error response
            if (user == null)
            {
                return new ServiceResponse(false, "User not found.");
            }

            // Update user settings
            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;

            // Attempt to update user
            var result = await userManager.UpdateAsync(user);

            // If user update operation failed, return an error response
            if (!result.Succeeded)
            {
                return new ServiceResponse(false, "Failed to update user settings.");
            }

            // User settings updated successfully, return a success response
            return new ServiceResponse(true, "User settings updated successfully.");
        }



        /// <summary>
        /// Asynchronously deletes a user account along with associated data such as blogs, comments, reactions, notifications, and histories.
        /// </summary>
        /// <param name="userId">The ID of the user account to delete.</param>
        /// <returns>A service response indicating the result of the account deletion operation.</returns>
        public async Task<ServiceResponse> DeleteAccountAsync(string userId)
        {
            // Find the user by ID
            var user = await FindUserById(userId.ToString());

            // If user is not found, return an error response
            if (user == null)
            {
                return new ServiceResponse(false, "User not found.");
            }

            // Find and delete all blogs authored by the user
            var blogs = await context.Blogs
                .Include(b => b.Reactions)
                .Include(b => b.Comments)
                .Include(b => b.Images)
                .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToLower()))
                .ToListAsync();

            foreach (var blog in blogs)
            {
                // Delete reactions associated with the blog
                if (blog.Reactions != null)
                {
                    context.BlogReactions.RemoveRange(blog.Reactions);
                }

                // Delete images associated with the blog
                if (blog.Images != null)
                {
                    context.BlogImages.RemoveRange(blog.Images);
                }

                // Find and delete comments associated with the blog
                if (blog.Comments != null)
                {
                    foreach (var item in blog.Comments)
                    {
                        var comment = await context.BlogComments
                            .Include(comment => comment.Reactions)
                            .FirstOrDefaultAsync(_ => _.BlogId == item.BlogId && _.UserId == item.UserId);

                        if (comment != null && comment.Reactions != null)
                        {
                            context.BlogCommentReactions.RemoveRange(comment.Reactions);
                        }

                        context.BlogComments.Remove(comment);
                    }
                }

                context.Blogs.Remove(blog);
            }

            await context.SaveChangesAsync();

            // Find and delete all comments authored by the user
            var comments = await context.BlogComments
                .Include(b => b.Reactions)
                .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToLower()))
                .ToListAsync();

            foreach (var comment in comments)
            {
                // Delete reactions associated with the comment
                if (comment.Reactions != null)
                {
                    context.BlogCommentReactions.RemoveRange(comment.Reactions);
                }

                context.BlogComments.Remove(comment);
            }

            await context.SaveChangesAsync();

            // Find and delete all reactions authored by the user
            var reactions = await context.BlogReactions
                .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToLower()))
                .ToListAsync();

            context.BlogReactions.RemoveRange(reactions);
            await context.SaveChangesAsync();

            // Find and delete all reactions on comments authored by the user
            var commentReactions = await context.BlogCommentReactions
                .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToLower()) || b.CommentUserId.ToString().ToLower().Equals(user.Id.ToLower()))
                .ToListAsync();

            context.BlogCommentReactions.RemoveRange(commentReactions);
            await context.SaveChangesAsync();

            // Find and delete all histories related to the user
            var histories = await context.Histories
                .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToLower()))
                .ToListAsync();

            context.Histories.RemoveRange(histories);
            await context.SaveChangesAsync();

            // Find and delete all notifications related to the user
            var notifications = await context.Notifications
                .Where(b => b.UserId.ToString().ToLower().Equals(user.Id.ToLower()))
                .ToListAsync();

            context.Notifications.RemoveRange(notifications);
            await context.SaveChangesAsync();

            // Attempt to delete the user account
            var result = await userManager.DeleteAsync(user);

            // If user deletion operation failed, return an error response
            if (!result.Succeeded)
            {
                return new ServiceResponse(false, "Failed to delete user account.");
            }

            // Update user security stamp
            await userManager.UpdateSecurityStampAsync(user);

            // User account deleted successfully, return a success response
            return new ServiceResponse(true, "User account deleted successfully.");
        }


        /// <summary>
        /// Asynchronously retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <exception cref="Exception">Thrown when the user with the specified ID is not found.</exception>
        public async Task<ApplicationUser> GetUserById(string userId)
        {
            // Find the user by ID
            var user = await FindUserById(userId);

            // If user is not found, throw an exception
            if (user is null)
            {
                throw new Exception("User not found.");
            }
            else
            {
                return user;
            }
        }


        /// <summary>
        /// Asynchronously sends a password reset link to the user's email.
        /// </summary>
        /// <param name="email">The email address of the user requesting the password reset.</param>
        /// <param name="scheme">The scheme used in the URL (e.g., http, https).</param>
        /// <param name="host">The host name of the application.</param>
        /// <param name="port">The port number of the application.</param>
        /// <returns>A service response indicating the result of the password reset link sending operation.</returns>
        public async Task<ServiceResponse> ForgotPassword(string email, string scheme, string host, int port)
        {
            try
            {
                // Find the user by email
                var user = await FindUserByEmail(email);

                // If user is not found, return an error response
                if (user is null)
                {
                    return new ServiceResponse(false, "User Not Found");
                }

                // Generate a password reset token for the user
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                // Build the reset password link URL
                var uriBuilder = new UriBuilder();
                uriBuilder.Scheme = scheme;
                uriBuilder.Host = host;
                uriBuilder.Port = port;
                uriBuilder.Path = "/Account/ResetPassword"; // Path to the ResetPassword action

                // Add query parameters for token and email to the URL
                var query = new StringBuilder();
                query.Append($"?token={HttpUtility.UrlEncode(token)}");
                query.Append($"&email={user.Email}");
                uriBuilder.Query = query.ToString();

                var forgotPassswordLink = uriBuilder.Uri.ToString();

                // Compose the email message with the reset password link
                var message = new Message(new string[] { user.Email }, "Confirmation Email Link", forgotPassswordLink);

                // Send the email using the email service
                _emailService.SendEmail(message);

                // Return a success response indicating the password reset link has been sent
                return new ServiceResponse(true, "Password Reset Link sent to your email successfully.");
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs during the operation
                return new ServiceResponse(false, ex.Message);
            }
        }


        /// <summary>
        /// Asynchronously resets the password for a user using the provided token and new password.
        /// </summary>
        /// <param name="resetPassword">The DTO containing the email, token, and new password for the password reset.</param>
        /// <returns>A service response indicating the result of the password reset operation.</returns>
        public async Task<ServiceResponse> ResetPassword(ResetPasswordRequestDTO resetPassword)
        {
            // Find the user by email
            var user = await FindUserByEmail(resetPassword.Email);

            // If user is not found, return an error response
            if (user is null)
            {
                return new ServiceResponse(false, "User not found");
            }

            // Reset the password using the provided token and new password
            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

            // If password reset operation failed, return an error response with error descriptions
            if (!resetPassResult.Succeeded)
            {
                // Iterate over each error and collect their descriptions
                var errorDescriptions = resetPassResult.Errors.Select(error => error.Description).ToList();

                // Join the error descriptions into a single string
                var errorMessage = string.Join(", ", errorDescriptions);

                return new ServiceResponse(false, errorMessage);
            }

            // Password reset successfully, return a success response
            return new ServiceResponse(true, "Password Reset Successfully");
        }



    }

}
