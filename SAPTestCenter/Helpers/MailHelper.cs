using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SAPTestCenter.Helpers
{
    public class MailHelper
    {
        public static void SendMail(MailMessage MMsg)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            client.Host = "smtp3.hp.com";
            client.Send(MMsg);
        }

        public static void AddAdminMail(MailMessage Msg)
        {
            string mailList = ConfigurationManager.AppSettings["AdminMailList"];
            foreach (var mail in mailList.Split(';'))
            {
                Msg.Bcc.Add(mail);
            }

        }
    }
}