using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response.Dashboard
{
    public class DashboardDataDTO
    {
        [Required]
        public int BlogsCount { get; set; } = 0;
        [Required]

        public int UpvotesCount { get; set; } = 0;
        [Required]

        public int DownvotesCount { get; set; } = 0;
        [Required]

        public int CommentsCount { get; set; } = 0;
    }
}
