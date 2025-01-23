using System.Net.Mail;
using System.Net;

namespace ASM_C_4.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("duongpxps38124@gmail.com", "dtuistivzuvqnxfz")
            };

            return client.SendMailAsync(
                new MailMessage(from: "demologin979@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
