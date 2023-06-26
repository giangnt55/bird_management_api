using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Extensions
{
    public class MailExtension
    {
        public void SendMail(string name, string emailCus, string body)
        {
            

            // Tạo đối tượng email
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("birdweb@gmail.com"));
            email.To.Add(MailboxAddress.Parse(emailCus));
            email.Date = DateTime.Now;
            email.Subject = "Reset Password";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<h3>Hello "+ name +",</h3>"+ body,
            };

            // Gửi email
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("birdmanagement15@gmail.com", "gihvztkdonwrlmic");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
