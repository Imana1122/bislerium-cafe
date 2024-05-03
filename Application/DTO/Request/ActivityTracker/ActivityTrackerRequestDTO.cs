using Application.DTO.Response.ActivityTracker;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Request.ActivityTracker
{
    public class ActivityTrackerRequestDTO: BaseActivityTracker
    {
        [Required]
        public string UserName { get; set; }
    }
}
