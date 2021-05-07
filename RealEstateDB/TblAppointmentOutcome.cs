using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblAppointmentOutcome
    {
        public TblAppointmentOutcome()
        {
            TblLeadAppointments = new HashSet<TblLeadAppointment>();
        }

        public int AppointmentOutcomeId { get; set; }
        public int? AccountId { get; set; }
        public string AppointmentOutcomeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual ICollection<TblLeadAppointment> TblLeadAppointments { get; set; }
    }
}
