using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLeadFile
    {
        public int LeadFileId { get; set; }
        public int? LeadId { get; set; }
        public int? AccountId { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblLead Lead { get; set; }
    }
}
