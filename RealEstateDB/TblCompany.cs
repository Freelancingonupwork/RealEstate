using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblCompany
    {
        public TblCompany()
        {
            TblAgents = new HashSet<TblAgent>();
            TblLeadSources = new HashSet<TblLeadSource>();
            TblLeadTags = new HashSet<TblLeadTag>();
            TblLeads = new HashSet<TblLead>();
            TblStages = new HashSet<TblStage>();
            TblTags = new HashSet<TblTag>();
            TblUsers = new HashSet<TblUser>();
        }

        public int CompanyId { get; set; }
        public string FullName { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public int? LogionTypeId { get; set; }
        public bool? IsCompany { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblUserLoginType LogionType { get; set; }
        public virtual ICollection<TblAgent> TblAgents { get; set; }
        public virtual ICollection<TblLeadSource> TblLeadSources { get; set; }
        public virtual ICollection<TblLeadTag> TblLeadTags { get; set; }
        public virtual ICollection<TblLead> TblLeads { get; set; }
        public virtual ICollection<TblStage> TblStages { get; set; }
        public virtual ICollection<TblTag> TblTags { get; set; }
        public virtual ICollection<TblUser> TblUsers { get; set; }
    }
}
