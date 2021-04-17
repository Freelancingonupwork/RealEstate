using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLeadSource
    {
        public int LeadSourceId { get; set; }
        public int? CompanyId { get; set; }
        public string LeadSourceName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblCompany Company { get; set; }
    }
}
