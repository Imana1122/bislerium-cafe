using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response.Identity
{
    //response model to get user with claims
    public class GetUserWithClaimResponseDTO : BaseUserClaimDTO
    {
        public string Email { get; set; }
    }
}
