using RealEstateDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class TemplateCategoryHTMLEmailViewModel
    {
        public int TemplateCategoryHTMLEmailID { get; set; }
        public int? TemplateCategoryId { get; set; }
        public string TemplateHTMLImage { get; set; }
        public string TemplateHTMLEmail { get; set; }
        public string TemplateHTMLEmailDescription { get; set; }
        public virtual TblTemplateCategory TemplateCategory { get; set; }
    }

    public class TemplateCategoryHTMLEmailList
    {
        public List<TemplateCategoryHTMLDetails> TemplateCategoryHTMLDetails { get; set; }
    }

    public class TemplateCategoryHTMLDetails
    {
        public string CategotyName { get; set; }
        public List<TemplateCategoryHTMLEmailViewModel> TemplateCategoryHTMLEmailList { get; set; }
    }
}
