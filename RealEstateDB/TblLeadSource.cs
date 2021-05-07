using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLeadSource
    {
        public int LeadSourceId { get; set; }
        public int? AccountId { get; set; }
        public string LeadSourceName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
    }
}
