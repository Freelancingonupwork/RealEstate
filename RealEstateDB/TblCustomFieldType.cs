using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblCustomFieldType
    {
        public TblCustomFieldType()
        {
            TblCustomFields = new HashSet<TblCustomField>();
        }

        public int Id { get; set; }
        public string FieldType { get; set; }

        public virtual ICollection<TblCustomField> TblCustomFields { get; set; }
    }
}
