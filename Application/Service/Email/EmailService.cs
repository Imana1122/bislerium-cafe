using Application.Extensions.Email;
using Application.Interface.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Email
{
    public class EmailService(IEmail email) : IEmailService
    {
        public void SendEmail(Message message)
        {
             email.SendEmail(message);
        }
    }
}
