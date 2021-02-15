﻿using Kavenegar;
using Shop.Data.Context;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Service
{
    public class MessageSender : IMessageSender
    {
        private readonly IUnitOfWork<DatabaseContext> _db;

        public MessageSender(IUnitOfWork<DatabaseContext> db)
        {
            _db = db;
        }

        public void SendEmail(string To, string Subject, string Body)
        {
            var setting = _db.SettingRepository.Get().LastOrDefault();

            MailMessage message = new MailMessage();
            SmtpClient client = new SmtpClient("smtp.gmail.com");

            message.From = new MailAddress(setting.EmailAddress, setting.ShopName);
            message.To.Add(new MailAddress(To));
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = true;

            client.Port = 587;
            client.Credentials = new NetworkCredential(setting.EmailAddress, setting.EmailPassword);
            client.EnableSsl = true;

            client.Send(message);
        }

        public void SendSms(string To, string Body)
        {
            var setting = _db.SettingRepository.Get().LastOrDefault();

            var sender = setting.SmsSender;
            var receptor = To;
            var message = Body;
            var api = new KavenegarApi(setting.SmsApi);
            api.Send(sender, receptor, message);
        }
    }
}
