using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblLead
    {
        public TblLead()
        {
            TblCustomFieldAnswers = new HashSet<TblCustomFieldAnswer>();
            TblLeadAppointments = new HashSet<TblLeadAppointment>();
            TblLeadEmailMessages = new HashSet<TblLeadEmailMessage>();
            TblLeadFiles = new HashSet<TblLeadFile>();
            TblLeadTags = new HashSet<TblLeadTag>();
        }

        public int LeadId { get; set; }
        public int? AccountId { get; set; }
        public int? AgentId { get; set; }
        public int? StageId { get; set; }
        public int? CustomFieldId { get; set; }
        public string LeadSource { get; set; }
        public string LeadStatus { get; set; }
        public string Industry { get; set; }
        public string Stage { get; set; }
        public string OwnerImg { get; set; }
        public string LeadOwner { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string MobileNumber { get; set; }
        public string Website { get; set; }
        public int? NoOfEmp { get; set; }
        public decimal? AnnualRevenue { get; set; }
        public int? Rating { get; set; }
        public bool? EmailOptOut { get; set; }
        public string SkypeId { get; set; }
        public string TwitterId { get; set; }
        public string SecondaryEmail { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
        public virtual TblAccount Agent { get; set; }
        public virtual TblCustomField CustomField { get; set; }
        public virtual TblStage StageNavigation { get; set; }
        public virtual ICollection<TblCustomFieldAnswer> TblCustomFieldAnswers { get; set; }
        public virtual ICollection<TblLeadAppointment> TblLeadAppointments { get; set; }
        public virtual ICollection<TblLeadEmailMessage> TblLeadEmailMessages { get; set; }
        public virtual ICollection<TblLeadFile> TblLeadFiles { get; set; }
        public virtual ICollection<TblLeadTag> TblLeadTags { get; set; }
    }
}
