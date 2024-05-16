using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Request.Identity
{
    //request model for resetting password
    public class ResetPasswordRequestDTO
    {
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The password confirmation does not match")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
