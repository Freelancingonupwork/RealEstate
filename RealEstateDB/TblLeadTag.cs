using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLeadTag
    {
        public int LeadTagId { get; set; }
        public int? LeadId { get; set; }
        public int? TagId { get; set; }
        public int? CompanyId { get; set; }

        public virtual TblCompany Company { get; set; }
        public virtual TblLead Lead { get; set; }
        public virtual TblTag Tag { get; set; }
    }
}
