using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IMS.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body, true);
        }
        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml)
        {
            try
            {
                //using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
                //client.EnableSsl = _emailSettings.EnableSsl;
                //client.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword);

                //var mailMessage = new MailMessage
                //{
                //    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                //    Subject = subject,
                //    Body = body,
                //    IsBodyHtml = isHtml
                //};
                //mailMessage.To.Add(to);

                //await client.SendMailAsync(mailMessage);
                //_logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                // Don't throw in production, just log
            }
        }
        public async Task SendBulkEmailAsync(List<string> recipients, string subject, string body)
        {
            foreach (var recipient in recipients)
            {
                await SendEmailAsync(recipient, subject, body);
            }
        }
        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string fileName)
        {
            try
            {
                //using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
                //client.EnableSsl = _emailSettings.EnableSsl;
                //client.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword);

                //var mailMessage = new MailMessage
                //{
                //    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                //    Subject = subject,
                //    Body = body,
                //    IsBodyHtml = true
                //};
                //mailMessage.To.Add(to);

                //if (attachment != null && attachment.Length > 0)
                //{
                //    var stream = new MemoryStream(attachment);
                //    mailMessage.Attachments.Add(new Attachment(stream, fileName));
                //}

                //await client.SendMailAsync(mailMessage);
                //_logger.LogInformation($"Email with attachment sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email with attachment to {to}");
            }
        }
        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(to));

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    var attachment = new Attachment(attachmentPath);
                    message.Attachments.Add(attachment);
                }

                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    EnableSsl = _emailSettings.EnableSsl,
                    Timeout = _emailSettings.Timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
                };

                await client.SendMailAsync(message);
                _logger.LogInformation("Email with attachment sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email with attachment to {To}", to);
                throw new ApplicationException($"Failed to send email with attachment to {to}", ex);
            }
        }
    }
}