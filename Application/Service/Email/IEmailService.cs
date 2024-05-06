using Application.Extensions.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Email
{
    public interface IEmailService
    {
        void SendEmail(Message message);

    }
}
