using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response
{
    //response model for success or failure request result
    public record ServiceResponse(bool Flag, string Message);
   
}
