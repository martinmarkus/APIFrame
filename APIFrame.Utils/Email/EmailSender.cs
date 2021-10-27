using APIFrame.Utils.Email.DataObjects;
using APIFrame.Utils.Email.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace HyHeroesWebAPI.Presentation.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(
            EmailReceiver emailReceiver,
            MailOptions mailOptions,
            SmtpHost smtpHost)
        {
            var mailMessage = new MimeMessage();

            mailMessage.From.Add(new MailboxAddress(
                mailOptions.SenderName,
                mailOptions.SenderEmail));

            mailMessage.To.Add(new MailboxAddress(
                emailReceiver.ReceiverName,
                emailReceiver.ReceiverEmail));

            mailMessage.Subject = mailOptions.Subject;
            mailMessage.Body = new TextPart("html")
            {
                Text = mailOptions.BodyWithHtml
            };

            using var smtpClient = new SmtpClient();

            try
            {
                smtpClient.Connect(
                    smtpHost.Host,
                    int.Parse(smtpHost.Port),
                    bool.Parse(smtpHost.EnableSsl));

                smtpClient.Authenticate(
                    mailOptions.SenderEmail,
                    mailOptions.SenderPassword);

                await smtpClient.SendAsync(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                smtpClient.Disconnect(true);
            }
        }
    }
}
