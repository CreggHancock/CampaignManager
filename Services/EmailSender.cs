using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace DndManager.Services;
public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        SmtpClient client = new()
        {
            Port = 25,
            Host = "127.0.0.1",
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("test@dndmanager.com", "password")
        };

        return client.SendMailAsync("test@dndmanager.com", email, subject, htmlMessage);
    }
}
