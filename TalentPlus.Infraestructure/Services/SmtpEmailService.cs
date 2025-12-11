using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TalentPlus.Application.Interfaces;

namespace TalentPlus.Infraestructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string name)
    {
        // I retrieve SMTP settings from configuration
        var host = _configuration["Smtp:Host"];
        var port = int.Parse(_configuration["Smtp:Port"]);
        var user = _configuration["Smtp:User"];
        var pass = _configuration["Smtp:Password"];

        // I create the mail message with HTML content
        var mailMessage = new MailMessage
        {
            From = new MailAddress(user, "TalentPlus HR"),
            Subject = "¡Bienvenido a TalentPlus!",
            Body = $"<h1>Hola, {name}!</h1><p>Tu registro ha sido exitoso. Ya puedes ingresar a consultar tu información.</p>",
            IsBodyHtml = true,
        };
            
        mailMessage.To.Add(toEmail);

        // I configure the SMTP client to send the message securely
        using (var client = new SmtpClient(host, port))
        {
            client.Credentials = new NetworkCredential(user, pass);
            client.EnableSsl = true;
            await client.SendMailAsync(mailMessage);
        }
    }
}