using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblCustomFieldAnswer
    {
        public int CustomFieldAnsId { get; set; }
        public int? CustomFieldId { get; set; }
        public int? LeadId { get; set; }
        public int? AccountId { get; set; }
        public string FieldAns { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblCustomField CustomField { get; set; }
        public virtual TblLead Lead { get; set; }
    }
}
