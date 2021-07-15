using EAGetMail;
using EstajoMailService.App_Code.BAL;
using EstajoMailService.App_Code.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EstajoMailService
{
    class Program
    {

        // client configuration
        // You should create your client id and client secret,
        // do not use the following client id in production environment, it is used for test purpose only.
        static readonly string clientID = ConfigurationManager.AppSettings["GmailclientID"].ToString();
        static readonly string clientSecret = ConfigurationManager.AppSettings["GmailclientSecret"].ToString();
        static readonly string scope = ConfigurationManager.AppSettings["Gmailscope"].ToString(); //openid%20
        static readonly string authUri = ConfigurationManager.AppSettings["GmailauthUri"].ToString();
        static readonly string tokenUri = ConfigurationManager.AppSettings["GmailtokenUri"].ToString();


        static readonly string MicrosoftclientID = ConfigurationManager.AppSettings["MicrosoftclientID"].ToString();
        static readonly string MicrosoftclientSecret = ConfigurationManager.AppSettings["MicrosoftclientSecret"].ToString();
        static readonly string Microsoftscope = ConfigurationManager.AppSettings["Microsoftscope"].ToString(); //openid%20
        static readonly string MicrosoftauthUri = ConfigurationManager.AppSettings["MicrosoftauthUri"].ToString();
        static readonly string MicrosofttokenUri = ConfigurationManager.AppSettings["MicrosofttokenUri"].ToString();

        static readonly string ImagePath = ConfigurationManager.AppSettings["AttachmentImagePath"].ToString();
        static readonly string MailReadImgPath = ConfigurationManager.AppSettings["MailReadImgPath"].ToString();
        static async Task Main(string[] args)
        {
            try
            {
                // List<Gmail> MailLists = GetAllEmails(Convert.ToString(ConfigurationManager.AppSettings["HostAddress"]));
                //Console.WriteLine("+------------------------------------------------------------------+");
                //Console.WriteLine("  Sign in with Google                                             ");
                //Console.WriteLine("   If you got \"This app isn't verified\" information in Web Browser, ");
                //Console.WriteLine("   click \"Advanced\" -> Go to ... to continue test.");
                //Console.WriteLine("+------------------------------------------------------------------+");
                //Console.WriteLine("");
                //Console.WriteLine("Press any key to sign in...");
                //Console.ReadKey();
                utility.log("Gmail Service Started:-" + DateTime.Now);
                try
                {
                    using (var db = new RealEstateEntities())
                    {
                        Program p = new Program();

                        //string response1 = await p.DoOauthAndRetrieveRefreshtokenMicrosoft("M.R3_BAY.-CYU*OTLb*bB5E02FuQWE8eH!x2c9YnijegHItUdUwGKAkq!glywkGeatDSJDZD3bEGuLUomu1IVJG8sLZQKqgQ3hZeaz2yiWltW!cXxQnhcXSd8QxuMNK!pEAZPBN!xhBc7gHfLEQCgv!DqQHxYC1ySkkdOcOgFMZUdsbgP5p6QcZ3l99qdGrqrjvRm644tv8zxnYIIQ0Qzukua3ydx5ctqUaHUpoIO8tBMqCqMatVUZOShjyG1lUR87b96v3ketEZ2vN1!acZGc6*YncgdjvB4f3NYUP98K3CKvZ5pXybpZykRESyyFR!XUQ6Gn73vxUC1rh21cZzNc45krFvQqHUg$");

                        //p.DoOauthAndRetrieveEmail();
                        int AgentRoleId = RoleType.Agent.GetHashCode();
                        var oAgentList = db.tblAccounts.Where(x => x.RoleId.Value == AgentRoleId && x.IsEmailConfig == true).ToList();
                        if (oAgentList.Count > 0)
                        {
                            foreach (var item in oAgentList)
                            {
                                //tblAccountIntegration oData = db.tblAccountIntegrations.Where(x => x.AccountId == item.AccountId).FirstOrDefault();
                                //var oData = item.tblAccountIntegrations.Where(x => x.AccountId == item.AccountId).FirstOrDefault();
                                if (item.tblAccountIntegrations.Count >= 1)
                                {
                                    foreach (var itemAccountIntegrations in item.tblAccountIntegrations)
                                    {
                                        if (itemAccountIntegrations.AuthAccountType == AuthAccountType.GoogleAuth.GetHashCode())
                                        {
                                            DateTime myDate1 = itemAccountIntegrations.CreatedDate.Value;
                                            DateTime myDate2 = DateTime.Now;
                                            TimeSpan difference = myDate2.Subtract(myDate1);
                                            //if (DateTime.Now.Subtract(oData.CreatedDate.Value).Hours >= 1)
                                            if (difference.TotalHours >= 1)
                                            {
                                                string response = await p.DoOauthAndRetrieveRefreshtoken(itemAccountIntegrations.RefreshToken);
                                                //OAuthResponseParser parser = new OAuthResponseParser();
                                                //parser.Load(response);

                                                Rootobject result = JsonConvert.DeserializeObject<Rootobject>(response);

                                                if (result.access_token != "" && result.access_token != null)
                                                {
                                                    utility.log("Refreshtoken Sucessfully Generated for Gmail");
                                                    itemAccountIntegrations.AccessToken = result.access_token;
                                                    itemAccountIntegrations.CreatedDate = DateTime.Now;
                                                    db.SaveChanges();

                                                    //var UserEmail = db.tblLeads.Where(x => x.AgentId == item.AccountId).FirstOrDefault();
                                                    //if (UserEmail != null)
                                                    //{
                                                    //    RetrieveMailWithXOAUTH(oData.EmailAddress, result.access_token, UserEmail.EmailAddress, (int)oData.AccountId);
                                                    //}
                                                    //var UserList = db.tblLeads.Where(x => x.AgentId == item.AccountId).ToList();
                                                    if (item.tblLeads1.Count >= 1)
                                                    {
                                                        foreach (var itemUserList in item.tblLeads1)
                                                        {
                                                            var oLeadEmailMessage = item.tblLeadEmailMessages.Where(x => x.LeadId == itemUserList.LeadId && x.AccountId == itemUserList.AgentId && x.IsType == MessageType.EmailMessage.GetHashCode()).FirstOrDefault();
                                                            if (oLeadEmailMessage != null)
                                                            {
                                                                //RetrieveMailWithXOAUTH(itemAccountIntegrations.EmailAddress, itemAccountIntegrations.AccessToken /*result.access_token*/, itemUserList.EmailAddress, (int)itemAccountIntegrations.AccountId, oLeadEmailMessage.LeadEmailMessageId);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //string response = await p.DoOauthAndRetrieveRefreshtoken(oData.RefreshToken);
                                                //Rootobject result = JsonConvert.DeserializeObject<Rootobject>(response);

                                                if (itemAccountIntegrations.AccessToken != "" && itemAccountIntegrations.AccessToken != null)
                                                {
                                                    //RetrieveMailWithXOAUTH2(oData.EmailAddress, result.access_token);
                                                    //var UserEmail = db.tblLeads.Where(x => x.AgentId == item.AccountId).FirstOrDefault();
                                                    //if (UserEmail != null)
                                                    //{
                                                    //    RetrieveMailWithXOAUTH(oData.EmailAddress, oData.AccessToken /*result.access_token*/, UserEmail.EmailAddress, (int)oData.AccountId);
                                                    //}


                                                    //var UserList = db.tblLeads.Where(x => x.AgentId == item.AccountId).ToList();
                                                    if (item.tblLeads1.Count >= 1)
                                                    {
                                                        foreach (var itemUserList in item.tblLeads1)
                                                        {
                                                            var oLeadEmailMessage = item.tblLeadEmailMessages.Where(x => x.LeadId == itemUserList.LeadId && x.AccountId == itemUserList.AgentId && x.IsReplay == false && x.EmailMessageId == 0 && x.IsType == MessageType.EmailMessage.GetHashCode()).ToList();
                                                            //if (oLeadEmailMessage != null)
                                                            foreach (var itemLeadEmailMessage in oLeadEmailMessage)
                                                            {
                                                                RetrieveMailWithXOAUTH(itemAccountIntegrations.EmailAddress, itemAccountIntegrations.AccessToken /*result.access_token*/, itemUserList.EmailAddress, (int)itemAccountIntegrations.AccountId, /*itemLeadEmailMessage.LeadEmailMessageId*/ itemLeadEmailMessage);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (itemAccountIntegrations.AuthAccountType == AuthAccountType.MicrosoftAuth.GetHashCode())
                                        {
                                            DateTime myDate1 = itemAccountIntegrations.CreatedDate.Value;
                                            DateTime myDate2 = DateTime.Now;
                                            TimeSpan difference = myDate2.Subtract(myDate1);
                                            //if (DateTime.Now.Subtract(oData.CreatedDate.Value).Hours >= 1)
                                            if (difference.TotalHours >= 1)
                                            {
                                                string response = await p.DoOauthAndRetrieveRefreshtokenMicrosoft(itemAccountIntegrations.RefreshToken);

                                                RootobjectMicrosoft result = JsonConvert.DeserializeObject<RootobjectMicrosoft>(response);

                                                if (result.access_token != "" && result.access_token != null)
                                                {
                                                    itemAccountIntegrations.AccessToken = result.access_token;
                                                    itemAccountIntegrations.CreatedDate = DateTime.Now;
                                                    db.SaveChanges();

                                                    var UserEmail = db.tblLeads.Where(x => x.AgentId == item.AccountId).FirstOrDefault();
                                                    if (UserEmail != null)
                                                    {
                                                        RetrieveMailWithXOAUTH2Microsoft(itemAccountIntegrations.EmailAddress, result.access_token, UserEmail.EmailAddress, (int)itemAccountIntegrations.AccountId);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //string response = await p.DoOauthAndRetrieveRefreshtoken(oData.RefreshToken);
                                                //Rootobject result = JsonConvert.DeserializeObject<Rootobject>(response);

                                                if (itemAccountIntegrations.AccessToken != "" && itemAccountIntegrations.AccessToken != null)
                                                {
                                                    //RetrieveMailWithXOAUTH2(oData.EmailAddress, result.access_token);
                                                    var UserEmail = db.tblLeads.Where(x => x.AgentId == item.AccountId).FirstOrDefault();
                                                    if (UserEmail != null)
                                                    {
                                                        RetrieveMailWithXOAUTH2Microsoft(itemAccountIntegrations.EmailAddress, itemAccountIntegrations.AccessToken /*result.access_token*/, UserEmail.EmailAddress, (int)itemAccountIntegrations.AccountId);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    utility.log("Gmail Or Office 365 account is not linked for this agent.");
                                }
                            }
                        }
                        else
                        {
                            utility.log("No Agent have configure mail service.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    utility.log(ex.ToString());
                    //Console.WriteLine(ep.ToString());
                }
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                utility.log(ex.ToString());
                //Console.WriteLine("Error: " + ex);
            }
        }


        // Generate an unqiue email file name based on date time
        static string _generateFileName(int sequence)
        {
            DateTime currentDateTime = DateTime.Now;
            return string.Format("{0}-{1:000}-{2:000}.html", //eml
                currentDateTime.ToString("yyyyMMddHHmmss", new CultureInfo("en-US")),
                currentDateTime.Millisecond,
                sequence);
        }

        void RetrieveMailWithXOAUTH2(string userEmail, string accessToken)
        {
            try
            {
                // Create a folder named "inbox" under current directory
                // to save the email retrieved.
                string localInbox = string.Format("{0}\\inbox", Directory.GetCurrentDirectory());
                // If the folder is not existed, create it.
                if (!Directory.Exists(localInbox))
                {
                    Directory.CreateDirectory(localInbox);
                }

                MailServer oServer = new MailServer("imap.gmail.com",
                        userEmail,
                        accessToken, // use access token as password
                        ServerProtocol.Imap4);

                // Set IMAP OAUTH 2.0
                oServer.AuthType = ServerAuthType.AuthXOAUTH2;
                // Enable SSL/TLS connection, most modern email server require SSL/TLS by default
                oServer.SSLConnection = true;
                // Set IMAP4 SSL Port
                oServer.Port = 993;

                MailClient oClient = new MailClient("TryIt");
                // Get new email only, if you want to get all emails, please remove this line
                oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.All;
                oClient.GetMailInfosParam.SenderContains = "constro.umbraco@gmail.com";

                Console.WriteLine("Connecting {0} ...", oServer.Server);
                oClient.Connect(oServer);

                MailInfo[] infos = oClient.GetMailInfos();
                //MailInfo[] infos = oClient.SearchMail("ALL FROM \"Constro\"");
                Console.WriteLine("Total {0} email(s)\r\n", infos.Length);

                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}; Flags: {3}",
                        info.Index, info.Size, info.UIDL, info.Flags);

                    // Receive email from email server
                    Mail oMail = oClient.GetMail(info);

                    Console.WriteLine("From: {0}", oMail.From.ToString());
                    Console.WriteLine("Subject: {0}\r\n", oMail.Subject);
                    Console.WriteLine("Body: {0}\r\n", oMail.HtmlBody);

                    // Generate an unqiue email file name based on date time.
                    string fileName = _generateFileName(i + 1);
                    string fullPath = string.Format("{0}\\{1}", localInbox, fileName);

                    // Save email to local disk
                    oMail.SaveAs(fullPath, true);

                    // Mark email as read to prevent retrieving this email again.
                    oClient.MarkAsRead(info, true);

                    // If you want to delete current email, please use Delete method instead of MarkAsRead
                    // oClient.Delete(info);
                }

                // Quit and expunge emails marked as deleted from server.
                oClient.Quit();
                Console.WriteLine("Completed!");
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }


        static void RetrieveMailWithXOAUTH(string userEmail, string accessToken, string SenderContainsEmail, int AccountId, /*int LeadEmailMessageId*/ tblLeadEmailMessage itemLeadEmailMessage)
        {
            try
            {
                //// Create a folder named "inbox" under current directory
                //// to save the email retrieved.
                //string localInbox = string.Format("{0}\\inbox", Directory.GetCurrentDirectory());
                //// If the folder is not existed, create it.
                //if (!Directory.Exists(localInbox))
                //{
                //    Directory.CreateDirectory(localInbox);
                //}

                MailServer oServer = new MailServer("imap.gmail.com",
                        userEmail,
                        accessToken, // use access token as password
                        ServerProtocol.Imap4);

                // Set IMAP OAUTH 2.0
                oServer.AuthType = ServerAuthType.AuthXOAUTH2;
                // Enable SSL/TLS connection, most modern email server require SSL/TLS by default
                oServer.SSLConnection = true;
                // Set IMAP4 SSL Port
                oServer.Port = 993;

                MailClient oClient = new MailClient("TryIt");
                // Get new email only, if you want to get all emails, please remove this line
                oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
                oClient.GetMailInfosParam.SenderContains = SenderContainsEmail;


                Console.WriteLine("Connecting {0} ...", oServer.Server);
                utility.log("Connecting Gmail {0} ..." + oServer.Server);
                oClient.Connect(oServer);

                MailInfo[] infos = oClient.GetMailInfos();
                //MailInfo[] infos = oClient.SearchMail("ALL FROM \"Constro\"");
                Console.WriteLine("Total {0} email(s)\r\n", infos.Length);
                utility.log("Total number of new email found from Gmail:-" + infos.Length);

                using (var db = new RealEstateEntities())
                {
                    //var oLeadEmailMessage = db.tblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.EmailMessageId == 0).ToList();
                    //foreach (var item in itemLeadEmailMessage)
                    //{
                    for (int i = 0; i < infos.Length; i++)
                    {
                        MailInfo info = infos[i];
                        Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}; Flags: {3}",
                            info.Index, info.Size, info.UIDL, info.Flags);


                        // Receive email from email server
                        Mail oMail = oClient.GetMail(info);

                        Console.WriteLine("From: {0}", oMail.From.ToString());
                        Console.WriteLine("To: {0}", oMail.To.ToString());
                        Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                        utility.log("From:-" + oMail.From.ToString());
                        utility.log("To:-" + oMail.To[0].ToString());
                        utility.log("Subject:-" + oMail.Subject);
                        utility.log("Subject1:-" + oMail.Subject.Replace("Re:", "").Replace("(Trial Version)", "").Trim());
                        //Console.WriteLine("Body: {0}\r\n", oMail.HtmlBody);
                        //Console.WriteLine("Text Body: {0}\r\n", oMail.TextBody);
                        string oRepSubj = oMail.Subject.Replace("Re:", "").Replace("(Trial Version)", "").Trim();
                        if (itemLeadEmailMessage.Subject.Equals(oRepSubj)) // && LeadEmailMessageId == item.LeadEmailMessageId
                        {
                            utility.log(itemLeadEmailMessage.Subject + "----" + oMail.Subject + "---Matched");

                            foreach (var itemAttachment in oMail.Attachments)
                            {
                                if (itemAttachment.ContentID != "")
                                {
                                    string matchString = Regex.Match(oMail.HtmlBody, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[0].Value;
                                    if (matchString != "")
                                    {
                                        if (matchString.Contains(itemAttachment.ContentID))
                                        {
                                            if (!Directory.Exists(MailReadImgPath))
                                            {
                                                Directory.CreateDirectory(MailReadImgPath);
                                            }
                                            string fileName = Guid.NewGuid().ToString("N").Substring(0, 8) + Path.GetExtension(itemAttachment.Name);
                                            File.WriteAllBytes(MailReadImgPath + "\\" + fileName, itemAttachment.Content); // Requires System.IO

                                            oMail.HtmlBody = Regex.Replace(oMail.HtmlBody, matchString, @"<img src='../../mail-read-img/" + fileName + @"'/>");
                                        }
                                    }
                                }
                            }

                            tblLeadEmailMessage obj = new tblLeadEmailMessage();
                            obj.AccountId = AccountId;
                            obj.LeadId = itemLeadEmailMessage.LeadId;
                            foreach (var itemTo in oMail.To)
                            {
                                obj.ToName = itemTo.Name == "" ? itemTo.Address.Split('@')[0] : itemTo.Name;
                            }
                            obj.FromName = oMail.From.Name;
                            obj.Subject = itemLeadEmailMessage.Subject;
                            obj.Body = oMail.HtmlBody;
                            obj.IsReplay = true;
                            obj.IsRead = false;
                            obj.EmailMessageId = itemLeadEmailMessage.LeadEmailMessageId;
                            obj.CreatedDate = oMail.ReceivedDate;
                            db.tblLeadEmailMessages.Add(obj);
                            db.SaveChanges();


                            var oLeadEmailMessage = db.tblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.LeadEmailMessageId == itemLeadEmailMessage.LeadEmailMessageId && x.IsType == MessageType.EmailMessage.GetHashCode()).FirstOrDefault();
                            if (oLeadEmailMessage != null)
                            {
                                oLeadEmailMessage.IsRead = false;
                                db.SaveChanges();
                            }

                            foreach (var itemAttachment in oMail.Attachments)
                            {
                                if (itemAttachment.ContentID == "")
                                {
                                    string fileName = Guid.NewGuid().ToString("N").Substring(0, 8) + Path.GetExtension(itemAttachment.Name);
                                    //FileStream filestream = new FileStream(ImagePath + "\\" + item.AccountId + "\\" + item.LeadId + "\\" + fileName, FileMode.Create);
                                    //var streamwriter = new StreamWriter(filestream);
                                    //streamwriter.AutoFlush = true;
                                    //Console.SetOut(streamwriter);
                                    //Console.SetError(streamwriter);

                                    File.WriteAllBytes(ImagePath + "\\" + itemLeadEmailMessage.AccountId + "\\" + itemLeadEmailMessage.LeadId + "\\" + fileName, itemAttachment.Content); // Requires System.IO

                                    tblLeadEmailMessageAttachment oData = new tblLeadEmailMessageAttachment();
                                    //oData.LeadEmailMessageId = item.LeadEmailMessageId;
                                    oData.LeadEmailMessageId = obj.LeadEmailMessageId;
                                    oData.Attachement = string.IsNullOrEmpty(fileName) ? string.Empty : fileName;
                                    oData.CreatedDate = DateTime.Now;
                                    db.tblLeadEmailMessageAttachments.Add(oData);
                                    db.SaveChanges();
                                }
                            }

                            // Mark email as read to prevent retrieving this email again.
                            oClient.MarkAsRead(info, true);

                        }

                        // If you want to delete current email, please use Delete method instead of MarkAsRead
                        // oClient.Delete(info);
                    }
                    //}
                }
                // Quit and expunge emails marked as deleted from server.
                oClient.Quit();
                utility.log("Gmail Service completed:-" + DateTime.Now);
                utility.log("===================");
                //Console.WriteLine("Completed!");
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }

        static void RetrieveMailWithXOAUTH2Microsoft(string userEmail, string accessToken, string SenderContainsEmail, int AccountId)
        {
            try
            {

                // Hotmail/Outlook/LIVE Imap4 Server
                MailServer oServer = new MailServer("imap-mail.outlook.com",
                        userEmail,
                        accessToken, // use access token as password
                        ServerProtocol.Imap4);

                // Set IMAP OAUTH 2.0
                oServer.AuthType = ServerAuthType.AuthXOAUTH2;
                // Enable SSL/TLS connection, most modern email server require SSL/TLS by default
                oServer.SSLConnection = true;
                // Set IMAP4 SSL Port
                oServer.Port = 993;

                MailClient oClient = new MailClient("TryIt");
                // Get new email only, if you want to get all emails, please remove this line
                oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
                oClient.GetMailInfosParam.SenderContains = SenderContainsEmail;

                Console.WriteLine("Connecting {0} ...", oServer.Server);
                oClient.Connect(oServer);

                MailInfo[] infos = oClient.GetMailInfos();
                Console.WriteLine("Total {0} email(s)\r\n", infos.Length);

                using (var db = new RealEstateEntities())
                {
                    var oLeadEmailMessage = db.tblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.EmailMessageId == 0).ToList();
                    foreach (var item in oLeadEmailMessage)
                    {
                        for (int i = 0; i < infos.Length; i++)
                        {
                            MailInfo info = infos[i];
                            Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                                info.Index, info.Size, info.UIDL);

                            // Receive email from email server
                            Mail oMail = oClient.GetMail(info);

                            Console.WriteLine("From: {0}", oMail.From.ToString());
                            Console.WriteLine("Subject: {0}\r\n", oMail.Subject);


                            if (item.Subject.Equals(oMail.Subject.Replace("Re:", "").Replace("(Trial Version)", "").Trim()))
                            {
                                tblLeadEmailMessage obj = new tblLeadEmailMessage();
                                obj.AccountId = AccountId;
                                obj.LeadId = item.LeadId;
                                obj.Subject = item.Subject;
                                obj.Body = oMail.HtmlBody;
                                obj.IsReplay = true;
                                obj.EmailMessageId = item.LeadEmailMessageId;
                                obj.CreatedDate = oMail.ReceivedDate;
                                db.tblLeadEmailMessages.Add(obj);
                                db.SaveChanges();
                            }


                            // Mark email as read to prevent retrieving this email again.
                            oClient.MarkAsRead(info, true);

                            // If you want to delete current email, please use Delete method instead of MarkAsRead
                            // oClient.Delete(info);
                        }
                    }
                }
                // Quit and expunge emails marked as deleted from server.
                oClient.Quit();
                Console.WriteLine("Completed!");
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }

        static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        async void DoOauthAndRetrieveEmail()
        {
            // Creates a redirect URI using an available port on the loopback address.
            string redirectUri = string.Format("http://127.0.0.1:{0}/", GetRandomUnusedPort());
            Console.WriteLine("redirect URI: " + redirectUri);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            Console.WriteLine("Listening ...");
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}",
                authUri,
                scope,
                Uri.EscapeDataString(redirectUri),//"urn:ietf:wg:oauth:2.0:oob",
                clientID
            );

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings the Console to Focus.
            BringConsoleToFront();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head></head><body>Please return to the app and close current window.</body></html>");
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                Console.WriteLine(string.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }

            if (context.Request.QueryString.Get("code") == null)
            {
                Console.WriteLine("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            Console.WriteLine("Authorization code: " + code);

            string responseText = await RequestAccessToken(code, redirectUri);
            Console.WriteLine(responseText);

            OAuthResponseParser parser = new OAuthResponseParser();
            parser.Load(responseText);

            var user = parser.EmailInIdToken;
            var accessToken = parser.AccessToken;

            Console.WriteLine("User: {0}", user);
            Console.WriteLine("AccessToken: {0}", accessToken);

            RetrieveMailWithXOAUTH2(user, accessToken);
        }

        async Task<string> DoOauthAndRetrieveRefreshtoken(string RefreshToken)
        {

            string responseText = await RequestRefreshToken(RefreshToken, tokenUri);
            Console.WriteLine(responseText);

            return responseText;
        }

        async Task<string> RequestAccessToken(string code, string redirectUri)
        {
            Console.WriteLine("Exchanging code for tokens...");

            // builds the  request
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&client_secret={3}&grant_type=authorization_code",
                code,
                Uri.EscapeDataString(redirectUri),
                clientID,
                clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenUri);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;

            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    return await reader.ReadToEndAsync();
                }

            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        Console.WriteLine("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            Console.WriteLine(responseText);
                        }
                    }
                }

                throw ex;
            }
        }

        async Task<string> RequestRefreshToken(string refresh_token, string redirectUri)
        {
            Console.WriteLine("Exchanging refresh_token for  access tokens...");

            // builds the  request //redirect_uri={0}&
            string tokenRequestBody = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token",
                //Uri.EscapeDataString("urn:ietf:wg:oauth:2.0:oob"),
                clientID,
                clientSecret,
                refresh_token
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(redirectUri);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            //tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;

            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    return await reader.ReadToEndAsync();
                }

            }
            catch (WebException ex)
            {
                string responseText = string.Empty;
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        Console.WriteLine("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            responseText = await reader.ReadToEndAsync();
                            Console.WriteLine(responseText);
                        }
                    }
                }

                return responseText;
                //throw ex;
            }
        }

        async Task<string> DoOauthAndRetrieveRefreshtokenMicrosoft(string RefreshToken)
        {

            string responseText = await RequestRefreshTokenMicrosoft(MicrosofttokenUri, RefreshToken);
            Console.WriteLine(responseText);

            return responseText;
        }
        async Task<string> RequestRefreshTokenMicrosoft(string redirectUri, string refresh_token)
        {
            Console.WriteLine("Exchanging refresh_token for  access tokens...");

            // builds the  request
            string tokenRequestBody = string.Format("redirect_uri={0}&client_id={1}&client_secret={2}&refresh_token={3}&grant_type=refresh_token",
                Uri.EscapeDataString(redirectUri),
                MicrosoftclientID,
                MicrosoftclientSecret,
                refresh_token
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(MicrosofttokenUri);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;

            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    return await reader.ReadToEndAsync();
                }

            }
            catch (WebException ex)
            {
                string responseText = string.Empty;
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        Console.WriteLine("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            responseText = await reader.ReadToEndAsync();
                            Console.WriteLine(responseText);
                        }
                    }
                }

                return responseText;
                //throw ex;
            }
        }

        // Hack to bring the Console window to front.

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}
