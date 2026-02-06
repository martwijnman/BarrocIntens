using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace BarrocIntens.Helpers
{
    internal class HelperEmail
    {
        public static void SendEmail(string toEmail, string subject, string body, string? pdfPath = null)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("EMAIL@jouwdomein.nl"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            if (!string.IsNullOrEmpty(pdfPath))
            {
                mail.Attachments.Add(new Attachment(pdfPath));
            }

            var smtp = new SmtpClient("smtp.jouwdomein.nl")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    "EMAIL@jouwdomein.nl",
                    "WACHTWOORD")
            };

            //smtp.Send(mail);
        }
    }
}
