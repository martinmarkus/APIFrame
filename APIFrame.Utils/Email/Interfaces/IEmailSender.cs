using APIFrame.Utils.Email.DataObjects;
using System.Threading.Tasks;

namespace APIFrame.Utils.Email.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailReceiver emailReceiver, MailOptions mailOptions, SmtpHost smtpHost);
    }
}
