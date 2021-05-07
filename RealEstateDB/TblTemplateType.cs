using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblTemplateType
    {
        public TblTemplateType()
        {
            TblEmailTemplates = new HashSet<TblEmailTemplate>();
        }

        public int TemplateTypeId { get; set; }
        public string TypeName { get; set; }

        public virtual ICollection<TblEmailTemplate> TblEmailTemplates { get; set; }
    }
}
