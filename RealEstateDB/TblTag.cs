using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblTag
    {
        public TblTag()
        {
            TblLeadTags = new HashSet<TblLeadTag>();
        }

        public int TagId { get; set; }
        public int? AccountId { get; set; }
        public string TagName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual ICollection<TblLeadTag> TblLeadTags { get; set; }
    }
}
