﻿using System;
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
        public int? CompanyId { get; set; }
        public string TagName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblCompany Company { get; set; }
        public virtual ICollection<TblLeadTag> TblLeadTags { get; set; }
    }
}
