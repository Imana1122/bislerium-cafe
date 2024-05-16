using Application.Extensions.Email;
using MimeKit;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using NETCore.MailKit;
using Application.Interface.Emails;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Represents a service for sending emails.
    /// </summary>
    public class Email : IEmail
    {
        private readonly EmailConfiguration _emailConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="Email"/> class.
        /// </summary>
        /// <param name="emailConfig">The email configuration settings.</param>
        public Email(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        /// <summary>
        /// Sends an email message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        /// <summary>
        /// Creates a MimeMessage from the provided message details.
        /// </summary>
        /// <param name="message">The message details.</param>
        /// <returns>A MimeMessage representing the email.</returns>
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        /// <summary>
        /// Sends the provided MimeMessage using SMTP.
        /// </summary>
        /// <param name="mailMessage">The MimeMessage to send.</param>
        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
                Console.WriteLine("Email Sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
