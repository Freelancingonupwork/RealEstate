using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblAgent
    {
        public TblAgent()
        {
            TblLeadAppointments = new HashSet<TblLeadAppointment>();
            TblLeads = new HashSet<TblLead>();
        }

        public int Id { get; set; }
        public int? AccountId { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string CellPhone { get; set; }
        public string Password { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual ICollection<TblLeadAppointment> TblLeadAppointments { get; set; }
        public virtual ICollection<TblLead> TblLeads { get; set; }
    }
}
