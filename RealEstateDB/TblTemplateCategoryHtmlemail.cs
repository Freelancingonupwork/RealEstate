using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblTemplateCategoryHtmlemail
    {
        public int TemplateCategoryHtmlemailId { get; set; }
        public int? TemplateCategoryId { get; set; }
        public string TemplateHtmlimage { get; set; }
        public string TemplateHtmlemail { get; set; }
        public string TemplateHtmlemailDescription { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblTemplateCategory TemplateCategory { get; set; }
    }
}
