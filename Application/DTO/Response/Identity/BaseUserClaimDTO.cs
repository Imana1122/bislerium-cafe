using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response.Identity
{
    //model specifying claims for a user
    public class BaseUserClaimDTO
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string RoleName { get; set; }


        public bool Delete { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        
    }
}
