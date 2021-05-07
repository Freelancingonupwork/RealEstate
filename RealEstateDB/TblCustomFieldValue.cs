using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblCustomFieldValue
    {
        public int CustomFieldValueId { get; set; }
        public int? CustomFieldId { get; set; }
        public int? AccountId { get; set; }
        public string FieldValue { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblCustomField CustomField { get; set; }
    }
}
