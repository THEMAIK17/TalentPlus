namespace TalentPlus.Application.Interfaces;

public interface IEmailService
{
    // I define the contract to send an email asynchronously
    Task SendWelcomeEmailAsync(string toEmail, string name);
}