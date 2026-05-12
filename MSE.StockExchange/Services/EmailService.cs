using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MSE.StockExchange.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        try
        {
            var host = _configuration["SmtpSettings:Host"];
            var portString = _configuration["SmtpSettings:Port"];
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            var useSslString = _configuration["SmtpSettings:UseSsl"];
            var fromEmail = _configuration["SmtpSettings:FromEmail"];
            var fromName = _configuration["SmtpSettings:FromName"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(fromEmail))
            {
                _logger.LogWarning("SMTP settings are not configured. Email will not be sent. To: {To}, Subject: {Subject}", to, subject);
                return;
            }

            int port = int.TryParse(portString, out var p) ? p : 587;
            bool useSsl = bool.TryParse(useSslString, out var ssl) && ssl;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = useSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(to);

            // Log that we're sending (helpful for development with MailHog/smtp4dev or localhost)
            _logger.LogInformation("Sending email to {To} with subject '{Subject}'", to, subject);

            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending an email. To: {To}, Subject: {Subject}", to, subject);
            // In a production app, we might throw or handle it gracefully.
            // We'll swallow it here so login doesn't crash completely if SMTP is down, 
            // but the user won't get their OTP.
        }
    }
}
