using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblAgent
    {
        public TblAgent()
        {
            TblLeads = new HashSet<TblLead>();
        }

        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string CellPhone { get; set; }
        public string Password { get; set; }
        public int? UserLoginTypeId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblCompany Company { get; set; }
        public virtual TblUserLoginType UserLoginType { get; set; }
        public virtual ICollection<TblLead> TblLeads { get; set; }
    }
}
