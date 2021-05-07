using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblAppointmentType
    {
        public TblAppointmentType()
        {
            TblLeadAppointments = new HashSet<TblLeadAppointment>();
        }

        public int AppointmenTypeId { get; set; }
        public int? AccountId { get; set; }
        public string AppointmentTypeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual ICollection<TblLeadAppointment> TblLeadAppointments { get; set; }
    }
}
