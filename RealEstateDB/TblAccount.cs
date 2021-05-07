using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblAccount
    {
        public TblAccount()
        {
            TblAccountCompanies = new HashSet<TblAccountCompany>();
            TblAccountIntegrations = new HashSet<TblAccountIntegration>();
            TblAgents = new HashSet<TblAgent>();
            TblAppointmentOutcomes = new HashSet<TblAppointmentOutcome>();
            TblAppointmentTypes = new HashSet<TblAppointmentType>();
            TblCustomFieldAnswers = new HashSet<TblCustomFieldAnswer>();
            TblCustomFieldValues = new HashSet<TblCustomFieldValue>();
            TblCustomFields = new HashSet<TblCustomField>();
            TblEmailTemplates = new HashSet<TblEmailTemplate>();
            TblLeadAppointments = new HashSet<TblLeadAppointment>();
            TblLeadSources = new HashSet<TblLeadSource>();
            TblLeadTags = new HashSet<TblLeadTag>();
            TblLeads = new HashSet<TblLead>();
            TblStages = new HashSet<TblStage>();
            TblTags = new HashSet<TblTag>();
        }

        public int AccountId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsOwner { get; set; }
        public bool? Status { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblRole Role { get; set; }
        public virtual ICollection<TblAccountCompany> TblAccountCompanies { get; set; }
        public virtual ICollection<TblAccountIntegration> TblAccountIntegrations { get; set; }
        public virtual ICollection<TblAgent> TblAgents { get; set; }
        public virtual ICollection<TblAppointmentOutcome> TblAppointmentOutcomes { get; set; }
        public virtual ICollection<TblAppointmentType> TblAppointmentTypes { get; set; }
        public virtual ICollection<TblCustomFieldAnswer> TblCustomFieldAnswers { get; set; }
        public virtual ICollection<TblCustomFieldValue> TblCustomFieldValues { get; set; }
        public virtual ICollection<TblCustomField> TblCustomFields { get; set; }
        public virtual ICollection<TblEmailTemplate> TblEmailTemplates { get; set; }
        public virtual ICollection<TblLeadAppointment> TblLeadAppointments { get; set; }
        public virtual ICollection<TblLeadSource> TblLeadSources { get; set; }
        public virtual ICollection<TblLeadTag> TblLeadTags { get; set; }
        public virtual ICollection<TblLead> TblLeads { get; set; }
        public virtual ICollection<TblStage> TblStages { get; set; }
        public virtual ICollection<TblTag> TblTags { get; set; }
    }
}
