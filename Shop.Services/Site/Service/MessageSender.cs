using Kavenegar;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Service
{
    public class MessageSender : IMessageSender
    {
        public void SendEmail(string To, string Subject, string Body)
        {
            MailMessage message = new MailMessage();
            SmtpClient client = new SmtpClient("smtp.gmail.com");

            message.From = new MailAddress("", "فروشگاه اینترنتی");
            message.To.Add(new MailAddress(To));
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = true;

            client.Port = 587;
            client.Credentials = new NetworkCredential("", "");
            client.EnableSsl = true;

            client.Send(message);
        }

        public void SendSms(string To, string Body)
        {
            var sender = "";
            var receptor = To;
            var message = Body;
            var api = new KavenegarApi("");
            api.Send(sender, receptor, message);
        }
    }
}
