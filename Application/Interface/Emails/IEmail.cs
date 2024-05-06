using Application.Extensions.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Emails
{
    public interface IEmail
    {
        void SendEmail(Message message);

    }
}
