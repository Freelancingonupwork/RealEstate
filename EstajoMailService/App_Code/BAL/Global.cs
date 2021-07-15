using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstajoMailService.App_Code.BAL
{
    public enum RoleType
    {
        Admin = 1,
        Agent = 2,
        MicrosoftAccount = 3,
        Manual = 4,
        Company = 5
    }

    public enum AuthAccountType
    {
        HubSportAuth = 1,
        GoogleAuth = 2,
        MicrosoftAuth = 3
    }

    public class Rootobject
    {
        public string error { get; set; }
        public string error_description { get; set; }

        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }
    }

    public enum MessageType
    {
        EmailMessage = 1,
        TextMessage = 2
    }

    public class RootobjectMicrosoft
    {
        public string token_type { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }
}
