using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class Token
    {
        public string refresh_token { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public status Status { get; set; }
        public string message { get; set; }
        public string correlationId { get; set; }
    }

    public enum status
    {
        MISMATCH_REDIRECT_URI_AUTH_CODE = 1,
        EXPIRED_AUTH_CODE = 2,
        BAD_REDIRECT_URI=3
    }
}
