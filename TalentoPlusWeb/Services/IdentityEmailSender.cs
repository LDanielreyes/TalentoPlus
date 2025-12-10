using Microsoft.AspNetCore.Identity.UI.Services;
using TalentoPlus.Application.Services;

namespace TalentoPlusWeb.Services;

public class IdentityEmailSender(IEmailService emailService) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await emailService.SendEmailAsync(email, subject, htmlMessage);
    }
}
