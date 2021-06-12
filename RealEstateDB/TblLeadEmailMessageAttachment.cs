using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLeadEmailMessageAttachment
    {
        public int LeadEmailMessageAttachmentId { get; set; }
        public int? LeadEmailMessageId { get; set; }
        public string Attachement { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblLeadEmailMessage LeadEmailMessage { get; set; }
    }
}
