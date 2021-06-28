using EASendMail;
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
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                message.From = new System.Net.Mail.MailAddress(FromMail);
                message.To.Add(new System.Net.Mail.MailAddress(toMail));
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

        public static void SendMailUsingGmail(string EmailAddress, string AccessToken, string ToMailAddress, string EmailSubject, string MailBody , List<string> fileName)
        {
            // Gmail SMTP server address
            SmtpServer oServer = new SmtpServer("smtp.gmail.com");
            // enable SSL connection
            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
            // Using 587 port, you can also use 465 port
            oServer.Port = 587;

            // use Gmail SMTP OAUTH 2.0 authentication
            oServer.AuthType = SmtpAuthType.XOAUTH2;
            // set user authentication
            oServer.User = EmailAddress;// oAccountIntegration.EmailAddress;
            // use access token as password
            oServer.Password = AccessToken;// oAccountIntegration.AccessToken;

            SmtpMail oMail = new SmtpMail("TryIt");
            // Your Gmail email address
            oMail.From = EmailAddress;// oAccountIntegration.EmailAddress;

            // Please change recipient address to yours for test
            oMail.To = ToMailAddress;

            oMail.Subject = EmailSubject;
            oMail.HtmlBody = MailBody;
            foreach (var item in fileName)
            {
                oMail.AddAttachment(item);
            }

            EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();
            oSmtp.SendMail(oServer, oMail);
        }

        public static void SendMailUsingoffice365(string EmailAddress, string AccessToken, string ToMailAddress, string EmailSubject, string MailBody)
        {
            // Office365 server address
            SmtpServer oServer = new SmtpServer("outlook.office365.com");

            // Using 587 port, you can also use 465 port
            oServer.Port = 587;
            // enable SSL connection
            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

            // use SMTP OAUTH 2.0 authentication
            oServer.AuthType = SmtpAuthType.XOAUTH2;
            // set user authentication
            oServer.User = EmailAddress;
            // use access token as password
            oServer.Password = AccessToken;

            SmtpMail oMail = new SmtpMail("TryIt");
            // Your email address
            oMail.From = EmailAddress;

            // Please change recipient address to yours for test
            oMail.To = ToMailAddress;

            oMail.Subject = EmailSubject;
            oMail.TextBody = MailBody;

            Console.WriteLine("start to send email using OAUTH 2.0 ...");

            EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();
            oSmtp.SendMail(oServer, oMail);
        }

        public static void SendMailAccessToken(string FromEmailAddress, string accessToken, string ToEmailAddress,string Subject,string Body)
        {
            // Gmail SMTP server address
            SmtpServer oServer = new SmtpServer("smtp.gmail.com");
            // enable SSL connection
            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
            // Using 587 port, you can also use 465 port
            oServer.Port = 587;

            // use Gmail SMTP OAUTH 2.0 authentication
            oServer.AuthType = SmtpAuthType.XOAUTH2;
            // set user authentication
            oServer.User = FromEmailAddress;
            // use access token as password
            oServer.Password = accessToken;

            SmtpMail oMail = new SmtpMail("TryIt");
            // Your gmail email address
            oMail.From = FromEmailAddress;
            oMail.To = ToEmailAddress;

            oMail.Subject = Subject;
            oMail.HtmlBody = Body;
            //oMail.TextBody = Body;

            EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();
            oSmtp.SendMail(oServer, oMail);
        }

    }
}
