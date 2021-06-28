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
            TblAppointmentOutcomes = new HashSet<TblAppointmentOutcome>();
            TblAppointmentTypes = new HashSet<TblAppointmentType>();
            TblCustomFieldAnswers = new HashSet<TblCustomFieldAnswer>();
            TblCustomFieldValues = new HashSet<TblCustomFieldValue>();
            TblCustomFields = new HashSet<TblCustomField>();
            TblEmailTemplates = new HashSet<TblEmailTemplate>();
            TblLeadAccounts = new HashSet<TblLead>();
            TblLeadAgents = new HashSet<TblLead>();
            TblLeadAppointmentAccounts = new HashSet<TblLeadAppointment>();
            TblLeadAppointmentAgents = new HashSet<TblLeadAppointment>();
            TblLeadEmailMessages = new HashSet<TblLeadEmailMessage>();
            TblLeadFiles = new HashSet<TblLeadFile>();
            TblLeadSources = new HashSet<TblLeadSource>();
            TblLeadTags = new HashSet<TblLeadTag>();
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
        public bool? IsEmailConfig { get; set; }
        public bool? IsTempPassword { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblRole Role { get; set; }
        public virtual ICollection<TblAccountCompany> TblAccountCompanies { get; set; }
        public virtual ICollection<TblAccountIntegration> TblAccountIntegrations { get; set; }
        public virtual ICollection<TblAppointmentOutcome> TblAppointmentOutcomes { get; set; }
        public virtual ICollection<TblAppointmentType> TblAppointmentTypes { get; set; }
        public virtual ICollection<TblCustomFieldAnswer> TblCustomFieldAnswers { get; set; }
        public virtual ICollection<TblCustomFieldValue> TblCustomFieldValues { get; set; }
        public virtual ICollection<TblCustomField> TblCustomFields { get; set; }
        public virtual ICollection<TblEmailTemplate> TblEmailTemplates { get; set; }
        public virtual ICollection<TblLead> TblLeadAccounts { get; set; }
        public virtual ICollection<TblLead> TblLeadAgents { get; set; }
        public virtual ICollection<TblLeadAppointment> TblLeadAppointmentAccounts { get; set; }
        public virtual ICollection<TblLeadAppointment> TblLeadAppointmentAgents { get; set; }
        public virtual ICollection<TblLeadEmailMessage> TblLeadEmailMessages { get; set; }
        public virtual ICollection<TblLeadFile> TblLeadFiles { get; set; }
        public virtual ICollection<TblLeadSource> TblLeadSources { get; set; }
        public virtual ICollection<TblLeadTag> TblLeadTags { get; set; }
        public virtual ICollection<TblStage> TblStages { get; set; }
        public virtual ICollection<TblTag> TblTags { get; set; }
    }
}
