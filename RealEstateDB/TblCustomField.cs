using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblCustomField
    {
        public TblCustomField()
        {
            TblCustomFieldAnswers = new HashSet<TblCustomFieldAnswer>();
            TblCustomFieldValues = new HashSet<TblCustomFieldValue>();
            TblLeads = new HashSet<TblLead>();
        }

        public int Id { get; set; }
        public int? FieldTypeId { get; set; }
        public int? AccountId { get; set; }
        public string FieldName { get; set; }
        public bool? HideIfEmpty { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblCustomFieldType FieldType { get; set; }
        public virtual ICollection<TblCustomFieldAnswer> TblCustomFieldAnswers { get; set; }
        public virtual ICollection<TblCustomFieldValue> TblCustomFieldValues { get; set; }
        public virtual ICollection<TblLead> TblLeads { get; set; }
    }
}
