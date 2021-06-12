using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLeadEmailMessage
    {
        public TblLeadEmailMessage()
        {
            TblLeadEmailMessageAttachments = new HashSet<TblLeadEmailMessageAttachment>();
        }

        public int LeadEmailMessageId { get; set; }
        public int? LeadId { get; set; }
        public int? AccountId { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsReplay { get; set; }
        public int? EmailMessageId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblLead Lead { get; set; }
        public virtual ICollection<TblLeadEmailMessageAttachment> TblLeadEmailMessageAttachments { get; set; }
    }
}
