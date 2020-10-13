using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace HlidacStatu.ClassificationRepair
{
    public interface IEmailService
    {
        Task SendEmailAsync(string[] recipients, string subject, string message, string replyTo);
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _emailSettings;

        public EmailService(
            IOptions<SmtpSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string[] recipients, string subject, string message, string replyTo)
        {
            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSettings.FromAddress));

                foreach (string recipient in recipients)
                {
                    mimeMessage.To.Add(new MailboxAddress(recipient));
                }
                mimeMessage.ReplyTo.Add(new MailboxAddress(replyTo));
                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(_emailSettings.Server);
                    // Note: only needed if the SMTP server requires authentication
                    //await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
#pragma warning restore CS0618 // Type or member is obsolete
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}