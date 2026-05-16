using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace OnlineExaminationSystem.Services
{
    /// <summary>
    /// Lightweight SMTP wrapper. If Web.config has no SmtpHost, the
    /// methods log to System.Diagnostics.Debug instead of throwing,
    /// so dev environments without SMTP keep working.
    /// </summary>
    public static class EmailService
    {
        public static void SendInviteEmail(
            string toAddress, string toName, string loginEmail, string setupLink)
        {
            string subject = "Set your password - Online Examination System";
            string body =
                "<div style='font-family:Segoe UI,Arial,sans-serif;color:#1a1a1a'>" +
                "<h2>Welcome, " + System.Web.HttpUtility.HtmlEncode(toName ?? "") + "</h2>" +
                "<p>Your account has been created. Use the button below to set your password.</p>" +
                "<p><b>Login Email:</b> " + System.Web.HttpUtility.HtmlEncode(loginEmail) + "</p>" +
                "<p><a href='" + setupLink + "' style='background:#2563eb;color:#fff;padding:10px 20px;border-radius:8px;text-decoration:none'>Set my password</a></p>" +
                "<p style='font-size:12px;color:#666'>If the button does not work, paste this link in your browser:<br/>" +
                System.Web.HttpUtility.HtmlEncode(setupLink) + "</p></div>";

            Send(toAddress, toName, subject, body);
        }

        public static void SendNotificationEmail(string toAddress, string toName, string subject, string body)
        {
            Send(toAddress, toName, subject, body);
        }

        private static void Send(string toAddress, string toName, string subject, string body)
        {
            string host = ConfigurationManager.AppSettings["SmtpHost"];
            if (string.IsNullOrWhiteSpace(host))
            {
                System.Diagnostics.Debug.WriteLine(
                    "[EmailService] SMTP not configured. To=" + toAddress + " Subject=" + subject);
                return;
            }

            int port = 587;
            int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out port);
            string user = ConfigurationManager.AppSettings["SmtpUser"];
            string pwd = ConfigurationManager.AppSettings["SmtpPassword"];
            bool ssl = string.Equals(ConfigurationManager.AppSettings["SmtpEnableSsl"], "true",
                                     StringComparison.OrdinalIgnoreCase);
            string fromAddr = ConfigurationManager.AppSettings["SmtpFromAddress"] ?? "no-reply@onlineexam.local";
            string fromName = ConfigurationManager.AppSettings["SmtpFromName"] ?? "Online Examination System";

            using (var msg = new MailMessage())
            {
                msg.From = new MailAddress(fromAddr, fromName);
                msg.To.Add(new MailAddress(toAddress, toName ?? toAddress));
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = ssl;
                    if (!string.IsNullOrEmpty(user))
                        client.Credentials = new NetworkCredential(user, pwd);
                    client.Send(msg);
                }
            }
        }
    }
}
