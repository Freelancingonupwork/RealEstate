using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RealEstate.Utills
{
    public class Utility
    {

        public static bool sendMail(string toMail, string body, string subject, string fromEmail, string SmtpUserName, string SmtpPassword, int SmtpPort, string SmtpServer, bool isSSL)
        {
            try
            {
                var FromMail = fromEmail;
                var userName = SmtpUserName;
                var password = SmtpPassword;
                var port = SmtpPort;
                var smtpServer = SmtpServer;
                var ssl = isSSL;
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromMail);
                message.To.Add(new MailAddress(toMail));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = body;
                smtp.Port = port;
                smtp.Host = smtpServer; //for gmail host  
                smtp.EnableSsl = ssl;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(userName, password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return true;
            }
            catch (Exception Ex)
            {
                ErrorLog.log(Ex);
                return false;
            }
        }
    }
}
