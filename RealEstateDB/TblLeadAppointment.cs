using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLeadAppointment
    {
        public int LeadAppointmentId { get; set; }
        public int? LeadId { get; set; }
        public int? AccountId { get; set; }
        public int? AgentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AppointmentTypeId { get; set; }
        public int? AppointmentOutcomesId { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public string Location { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblAccount Agent { get; set; }
        public virtual TblAppointmentOutcome AppointmentOutcomes { get; set; }
        public virtual TblAppointmentType AppointmentType { get; set; }
        public virtual TblLead Lead { get; set; }
    }
}
