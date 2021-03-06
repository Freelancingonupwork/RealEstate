using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class AccountIntegrationViewModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public string RefreshToken { get; set; }
        public string EmailAddress { get; set; }
        public int AuthAccountType { get; set; }
        public int Expires_In { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
