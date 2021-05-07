using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblEmailTemplate
    {
        public int EmailTemplateId { get; set; }
        public int? TemplateTypeId { get; set; }
        public int? AccountId { get; set; }
        public string EmailName { get; set; }
        public string EmailTemplateDescription { get; set; }
        public string FromEmail { get; set; }
        public string EmailSubject { get; set; }
        public string Body { get; set; }
        public int? IsType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblTemplateType TemplateType { get; set; }
    }
}
