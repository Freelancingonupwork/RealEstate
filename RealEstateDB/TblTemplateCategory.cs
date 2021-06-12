using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblTemplateCategory
    {
        public TblTemplateCategory()
        {
            TblTemplateCategoryHtmlemails = new HashSet<TblTemplateCategoryHtmlemail>();
        }

        public int TemplateCategoryId { get; set; }
        public string TemplateCategoryName { get; set; }

        public virtual ICollection<TblTemplateCategoryHtmlemail> TblTemplateCategoryHtmlemails { get; set; }
    }
}
