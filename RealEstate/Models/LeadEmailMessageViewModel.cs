using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class LeadEmailMessageViewModel
    {
        public int LeadEmailMessageId { get; set; }
        public int? LeadId { get; set; }
        public int? AccountId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public bool? IsReplay { get; set; }
        public int? EmailMessageId { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public List<LeadEmailMessageMainList> LeadEmailMessageMainList { get; set; }
        public List<LeadEmailMessageViewModel> LeadEmailMessageReplayList { get; set; }
    }

    public class LeadEmailMessageMainList
    {
        public int LeadEmailMessageId { get; set; }
        public int? LeadId { get; set; }
        public int AccountId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsReplay { get; set; }
        public int? EmailMessageId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class LeadEmailMessageReplayList
    {
        public int LeadEmailMessageId { get; set; }
        public int? LeadId { get; set; }
        public int AccountId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsReplay { get; set; }
        public int? EmailMessageId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
