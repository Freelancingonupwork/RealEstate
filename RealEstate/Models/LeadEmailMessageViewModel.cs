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
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public bool? IsReplay { get; set; }
        public bool? IsRead { get; set; }
        public List<LeadEmailMessageReplayAttachement> LeadEmailMessageReplayAttachement { get; set; }
        public int? EmailMessageId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public List<string> imgcontain { get; set; }

        public List<string> isAttachmentcontain { get; set; }

        //public List<LeadEmailMessageMainList> LeadEmailMessageMainList { get; set; }
        public List<LeadEmailMessageViewModel> LeadEmailMessageReplayList { get; set; }

        public List<LeadEmailMessageAttachement> LeadEmailMessageattachement { get; set; }
    }

    public class LeadEmailMessageAttachement
    {
        public int LeadEmailMessageId { get; set; }
        public string FileName { get; set; }
        public int LeadId { get; set; }
    }

    public class LeadEmailMessageReplayAttachement
    {
        public int LeadEmailMessageId { get; set; }
        public int LeadId { get; set; }
        public string FileName { get; set; }
    }

    public class LeadEmailDetails
    {
        public string LeadName { get; set; }
        public string AgentName { get; set; }
        public string StageName { get; set; }
        public string EmailAddress { get; set; }
        public string Phonenumber { get; set; }
        public List<LeadEmailMessageViewModel> LeadEmailMessageViewModel { get; set; }
    }
}
