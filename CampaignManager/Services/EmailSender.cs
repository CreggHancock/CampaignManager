using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace CampaignManager.Services;
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
            Credentials = new NetworkCredential("test@campaignmanager.com", "password")
        };

        return client.SendMailAsync("test@campaignmanager.com", email, subject, htmlMessage);
    }
}
