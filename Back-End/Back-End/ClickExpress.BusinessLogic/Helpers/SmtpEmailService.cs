using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClickExpress.BusinessLogic.Helpers
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly string _from;
        private readonly string _adminEmail;
        private readonly bool _enabled;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _logger = logger;
            var section = config.GetSection("Smtp");
            _host       = section["Host"]       ?? "smtp.gmail.com";
            _port       = int.Parse(section["Port"] ?? "587");
            _user       = section["User"]       ?? string.Empty;
            _pass       = section["Password"]   ?? string.Empty;
            _from       = section["From"]       ?? _user;
            _adminEmail = section["AdminEmail"] ?? "clickexpress.inc@gmail.com";
            _enabled    = bool.Parse(section["Enabled"] ?? "false");
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            if (!_enabled)
            {
                _logger.LogDebug("SMTP disabled. Would send to {To}: {Subject}", to, subject);
                return;
            }

            using var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_user, _pass),
                EnableSsl = true,
            };

            using var msg = new MailMessage
            {
                From       = new MailAddress(_from, "Click Express Inc"),
                Subject    = subject,
                Body       = htmlBody,
                IsBodyHtml = true,
            };
            msg.To.Add(to);

            try
            {
                await client.SendMailAsync(msg);
                _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
            }
        }

        public async Task SendLeadConfirmationAsync(string toEmail, string fullName, string origin, string destination, string equipment)
        {
            var subject = "Your freight inquiry has been received — Click Express";
            var body = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:0 auto;background:#0a0a0a;color:#fff;padding:32px;border-radius:8px"">
  <div style=""border-bottom:3px solid #CC0000;padding-bottom:16px;margin-bottom:24px"">
    <h1 style=""margin:0;font-size:22px;color:#CC0000"">Click Express Inc</h1>
  </div>
  <h2 style=""font-size:18px;margin:0 0 12px"">Hi {fullName}, we received your inquiry!</h2>
  <p style=""color:rgba(255,255,255,0.7);line-height:1.6"">
    Our dispatcher will contact you within <strong style=""color:#fff"">1 hour</strong> to discuss your load.
  </p>
  <div style=""background:#111;border:1px solid rgba(255,255,255,0.08);border-radius:6px;padding:16px;margin:20px 0"">
    <div style=""display:flex;gap:8px;margin-bottom:8px""><span style=""color:#CC0000;min-width:100px"">Route:</span><span>{origin} → {destination}</span></div>
    <div style=""display:flex;gap:8px""><span style=""color:#CC0000;min-width:100px"">Equipment:</span><span>{equipment}</span></div>
  </div>
  <p style=""color:rgba(255,255,255,0.5);font-size:12px"">
    Questions? Call us: <a href=""tel:+17862026599"" style=""color:#CC0000"">+1 786-202-6599</a>
  </p>
</div>";
            await SendAsync(toEmail, subject, body);
        }

        public async Task SendLeadAlertAsync(string fullName, string email, string phone, string origin, string destination, string equipment, string message)
        {
            var subject = $"[NEW LEAD] {fullName} — {origin} → {destination}";
            var body = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:0 auto"">
  <h2 style=""color:#CC0000"">New Lead Received</h2>
  <table style=""width:100%;border-collapse:collapse"">
    <tr><td style=""padding:6px 0;color:#666;width:120px"">Name:</td><td><strong>{fullName}</strong></td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Email:</td><td><a href=""mailto:{email}"">{email}</a></td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Phone:</td><td><a href=""tel:{phone}"">{phone}</a></td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Route:</td><td>{origin} → {destination}</td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Equipment:</td><td>{equipment}</td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Message:</td><td>{message}</td></tr>
  </table>
</div>";
            await SendAsync(_adminEmail, subject, body);
        }

        public async Task SendJobApplicationConfirmationAsync(string toEmail, string applicantName, string position)
        {
            var subject = $"Application received: {position} — Click Express";
            var body = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:0 auto;background:#0a0a0a;color:#fff;padding:32px;border-radius:8px"">
  <div style=""border-bottom:3px solid #CC0000;padding-bottom:16px;margin-bottom:24px"">
    <h1 style=""margin:0;font-size:22px;color:#CC0000"">Click Express Inc</h1>
  </div>
  <h2 style=""font-size:18px;margin:0 0 12px"">Hi {applicantName}, your application is in!</h2>
  <p style=""color:rgba(255,255,255,0.7);line-height:1.6"">
    We received your application for <strong style=""color:#fff"">{position}</strong>.<br>
    Our team will review it and reach out within <strong style=""color:#fff"">2–3 business days</strong>.
  </p>
  <p style=""color:rgba(255,255,255,0.5);font-size:12px"">
    Questions? Call us: <a href=""tel:+17862026599"" style=""color:#CC0000"">+1 786-202-6599</a>
  </p>
</div>";
            await SendAsync(toEmail, subject, body);
        }

        public async Task SendJobApplicationAlertAsync(string applicantName, string email, string phone, string position, string experience)
        {
            var subject = $"[NEW APPLICATION] {applicantName} — {position}";
            var body = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:0 auto"">
  <h2 style=""color:#CC0000"">New Job Application</h2>
  <table style=""width:100%;border-collapse:collapse"">
    <tr><td style=""padding:6px 0;color:#666;width:120px"">Name:</td><td><strong>{applicantName}</strong></td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Position:</td><td>{position}</td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Email:</td><td><a href=""mailto:{email}"">{email}</a></td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Phone:</td><td><a href=""tel:{phone}"">{phone}</a></td></tr>
    <tr><td style=""padding:6px 0;color:#666"">Experience:</td><td>{experience}</td></tr>
  </table>
</div>";
            await SendAsync(_adminEmail, subject, body);
        }

        public async Task SendPasswordResetAsync(string toEmail, string resetLink)
        {
            var subject = "Reset your password — Click Express";
            var body = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:0 auto;background:#0a0a0a;color:#fff;padding:32px;border-radius:8px"">
  <div style=""border-bottom:3px solid #CC0000;padding-bottom:16px;margin-bottom:24px"">
    <h1 style=""margin:0;font-size:22px;color:#CC0000"">Click Express Inc</h1>
  </div>
  <h2 style=""font-size:18px;margin:0 0 12px"">Password Reset Request</h2>
  <p style=""color:rgba(255,255,255,0.7);line-height:1.6"">
    We received a request to reset your password. Click the button below to set a new one.
    This link expires in <strong style=""color:#fff"">1 hour</strong>.
  </p>
  <div style=""text-align:center;margin:28px 0"">
    <a href=""{resetLink}"" style=""background:#CC0000;color:#fff;text-decoration:none;padding:12px 28px;border-radius:4px;font-weight:bold;font-size:15px;display:inline-block"">Reset Password</a>
  </div>
  <p style=""color:rgba(255,255,255,0.4);font-size:12px"">
    If you didn't request this, you can safely ignore this email. Your password will not change.
  </p>
</div>";
            await SendAsync(toEmail, subject, body);
        }
    }
}
