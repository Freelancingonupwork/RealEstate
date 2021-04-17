using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblStage
    {
        public TblStage()
        {
            TblLeads = new HashSet<TblLead>();
        }

        public int StageId { get; set; }
        public int? CompanyId { get; set; }
        public string StageName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblCompany Company { get; set; }
        public virtual ICollection<TblLead> TblLeads { get; set; }
    }
}
