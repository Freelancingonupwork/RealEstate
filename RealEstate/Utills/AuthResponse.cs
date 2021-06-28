using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Utills
{
    public class AuthResponse
    {
        private string access_token;
        public string Access_token
        {
            get
            {
                //// Access token lasts an hour if its expired we get a new one.
                //if (DateTime.Now.Subtract(created).Hours > 1)
                //{
                //    refresh();
                //}
                return access_token;
            }
            set { access_token = value; }
        }
        public string refresh_token { get; set; }
        public string clientId { get; set; }
        public string secret { get; set; }
        public string expires_in { get; set; }
        public DateTime created { get; set; }


        /// <summary>
        /// Parse the json response 
        /// //  "{\n  \"access_token\" : \"ya29.kwFUj-la2lATSkrqFlJXBqQjCIZiTg51GYpKt8Me8AJO5JWf0Sx6-0ZWmTpxJjrBrxNS_JzVw969LA\",\n  \"token_type\" : \"Bearer\",\n  \"expires_in\" : 3600,\n  \"refresh_token\" : \"1/ejoPJIyBAhPHRXQ7pHLxJX2VfDBRz29hqS_i5DuC1cQ\"\n}"
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static AuthResponse get(string response)
        {
            AuthResponse result = JsonConvert.DeserializeObject<AuthResponse>(response);
            result.created = DateTime.Now;   // DateTime.Now.Add(new TimeSpan(-2, 0, 0)); //For testing force refresh.
            return result;
        }

        public static AuthResponse refresh(string clientId, string GoogleoAuthTokenURL, string secret, string refresh_token)
        {
            var request = (HttpWebRequest)WebRequest.Create(GoogleoAuthTokenURL);
            string postData = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token", clientId, secret, refresh_token);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var refreshResponse = AuthResponse.get(responseString);

            return refreshResponse;
            //this.access_token = refreshResponse.access_token;
            //this.created = DateTime.Now;
        }


        public static AuthResponse Exchange(string authCode,string GoogleoAuthTokenURL, string clientid, string secret, string redirectURI)
        {

            var request = (HttpWebRequest)WebRequest.Create(GoogleoAuthTokenURL); //https://accounts.google.com/o/oauth2/token //"https://www.googleapis.com/oauth2/v4/token"

            string postData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", authCode, clientid, secret, redirectURI);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var x = AuthResponse.get(responseString);

            x.clientId = clientid;
            x.secret = secret;

            return x;

        }



        public static string GetAutenticationURI(string GoogleoAuthURL, string clientId, string redirectUri,string scopes)
        {
            ////string scopes = "https://www.googleapis.com/auth/plus.login email";
            //string scopes = "https://mail.google.com/+https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile"; //, 

            //if (string.IsNullOrEmpty(redirectUri))
            //{
            //    redirectUri = "urn:ietf:wg:oauth:2.0:oob";
            //}
            //string oauth = string.Format("https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&scope={2}&response_type=code&access_type=offline&approval_prompt=force", clientId, redirectUri, scopes);
            string oauth = string.Format(GoogleoAuthURL + "?client_id={0}&redirect_uri={1}&scope={2}&response_type=code&access_type=offline&approval_prompt=force&type=web_server&flowName=GeneralOAuthFlow", clientId, redirectUri, scopes);
            return oauth;
        }



    }



    //public class Rootobject
    //{
    //    public string id { get; set; }
    //    public string email { get; set; }
    //    public bool verified_email { get; set; }
    //    public string picture { get; set; }

    //    public string odatacontext { get; set; }
    //    public string displayName { get; set; }
    //    public string surname { get; set; }
    //    public string givenName { get; set; }
    //    //public string id { get; set; }
    //    public string userPrincipalName { get; set; }
    //    public object[] businessPhones { get; set; }
    //    public object jobTitle { get; set; }
    //    public object mail { get; set; }
    //    public object mobilePhone { get; set; }
    //    public object officeLocation { get; set; }
    //    public object preferredLanguage { get; set; }
    //}


    public class AuthResponseMicrosoft
    {
        public string token_type { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }


        public static AuthResponseMicrosoft getMicrosoft(string response)
        {
            AuthResponseMicrosoft result = JsonConvert.DeserializeObject<AuthResponseMicrosoft>(response);
            return result;
        }

        public static AuthResponseMicrosoft ExchangeMicrosoft(string authCode, string clientid, string secret, string redirectURI)
        {

            var request = (HttpWebRequest)WebRequest.Create("https://login.microsoftonline.com/common/oauth2/v2.0/token");

            string postData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", authCode, clientid, secret, redirectURI);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var x = AuthResponseMicrosoft.getMicrosoft(responseString);

            //x.clientId = clientid;
            //x.secret = secret;

            return x;

        }

        public static AuthResponseMicrosoft refreshTokenMicrosoft(string clientId, string secret, string refresh_token)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://login.microsoftonline.com/common/oauth2/v2.0/token");
            string postData = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token", clientId, secret, refresh_token);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var refreshResponse = AuthResponseMicrosoft.getMicrosoft(responseString);

            return refreshResponse;
            //this.access_token = refreshResponse.access_token;
            //this.created = DateTime.Now;
        }
        public static string GetAutenticationURIMicrosoft(string clientId, string redirectUri, string scopes, int AccountId)
        {
            //string scopes = "https://www.googleapis.com/auth/plus.login email";
            //string scopes = "openid+email+profile+Mail.Read+Mail.Send+Mail.ReadWrite+SMTP.Send+offline_access"; //, 
            string oauth = string.Format("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={0}&redirect_uri={1}&scope={2}&response_mode=form_post&prompt=select_account&response_type=code&state={3}", clientId, redirectUri, scopes, AccountId);
            return oauth;
        }

    }


    public class RootobjectGmail
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public string hd { get; set; }
    }


    public class RootobjectMicrosoft
    {
        public string odatacontext { get; set; }
        public string displayName { get; set; }
        public string surname { get; set; }
        public string givenName { get; set; }
        public string id { get; set; }
        public string userPrincipalName { get; set; }
        public object[] businessPhones { get; set; }
        public object jobTitle { get; set; }
        public object mail { get; set; }
        public object mobilePhone { get; set; }
        public object officeLocation { get; set; }
        public object preferredLanguage { get; set; }
    }


}
