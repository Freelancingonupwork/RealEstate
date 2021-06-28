using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace RealEstateDB
{
    public partial class RealEstateContext : DbContext
    {
        public RealEstateContext()
        {
        }

        public RealEstateContext(DbContextOptions<RealEstateContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAccount> TblAccounts { get; set; }
        public virtual DbSet<TblAccountCompany> TblAccountCompanies { get; set; }
        public virtual DbSet<TblAccountIntegration> TblAccountIntegrations { get; set; }
        public virtual DbSet<TblAppointmentOutcome> TblAppointmentOutcomes { get; set; }
        public virtual DbSet<TblAppointmentType> TblAppointmentTypes { get; set; }
        public virtual DbSet<TblCustomField> TblCustomFields { get; set; }
        public virtual DbSet<TblCustomFieldAnswer> TblCustomFieldAnswers { get; set; }
        public virtual DbSet<TblCustomFieldType> TblCustomFieldTypes { get; set; }
        public virtual DbSet<TblCustomFieldValue> TblCustomFieldValues { get; set; }
        public virtual DbSet<TblEmailTemplate> TblEmailTemplates { get; set; }
        public virtual DbSet<TblIndustry> TblIndustries { get; set; }
        public virtual DbSet<TblLead> TblLeads { get; set; }
        public virtual DbSet<TblLeadAppointment> TblLeadAppointments { get; set; }
        public virtual DbSet<TblLeadEmailMessage> TblLeadEmailMessages { get; set; }
        public virtual DbSet<TblLeadEmailMessageAttachment> TblLeadEmailMessageAttachments { get; set; }
        public virtual DbSet<TblLeadFile> TblLeadFiles { get; set; }
        public virtual DbSet<TblLeadSource> TblLeadSources { get; set; }
        public virtual DbSet<TblLeadStatus> TblLeadStatuses { get; set; }
        public virtual DbSet<TblLeadTag> TblLeadTags { get; set; }
        public virtual DbSet<TblRole> TblRoles { get; set; }
        public virtual DbSet<TblSmtp> TblSmtps { get; set; }
        public virtual DbSet<TblStage> TblStages { get; set; }
        public virtual DbSet<TblTag> TblTags { get; set; }
        public virtual DbSet<TblTemplateCategory> TblTemplateCategories { get; set; }
        public virtual DbSet<TblTemplateCategoryHtmlemail> TblTemplateCategoryHtmlemails { get; set; }
        public virtual DbSet<TblTemplateType> TblTemplateTypes { get; set; }
        public virtual DbSet<TblUserLoginType> TblUserLoginTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-VC6KLNP\\SQLEXPRESSSANJU;user id=sa;password=Potenza@123;Database=RealEstate;Trusted_Connection=True;");
                //optionsBuilder.UseSqlServer(@"Server=Estajo\SQLEXPRESS;user id=sa;password=estajo@123;Database=EstajoCRM;Trusted_Connection=False;MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TblAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PK_tblUser");

                entity.ToTable("tblAccount");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.IsEmailConfig).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsTempPassword)
                    .HasColumnName("isTempPassword")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TblAccounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblUser_tblRole");
            });

            modelBuilder.Entity<TblAccountCompany>(entity =>
            {
                entity.HasKey(e => e.AccountDetailsId)
                    .HasName("PK_tblCompany");

                entity.ToTable("tblAccountCompany");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.State).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.ZipCode).HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblAccountCompanies)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCompany_tblCompany");
            });

            modelBuilder.Entity<TblAccountIntegration>(entity =>
            {
                entity.ToTable("tblAccountIntegration");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(50);

                entity.Property(e => e.ExpiresIn).HasColumnName("Expires_In");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblAccountIntegrations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblAccountIntegration_tblAccount");
            });

            modelBuilder.Entity<TblAppointmentOutcome>(entity =>
            {
                entity.HasKey(e => e.AppointmentOutcomeId);

                entity.ToTable("tblAppointmentOutcomes");

                entity.Property(e => e.AppointmentOutcomeName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblAppointmentOutcomes)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblAppointmentOutcomes_tblAccount");
            });

            modelBuilder.Entity<TblAppointmentType>(entity =>
            {
                entity.HasKey(e => e.AppointmenTypeId);

                entity.ToTable("tblAppointmentTypes");

                entity.Property(e => e.AppointmentTypeName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblAppointmentTypes)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblAppointmentTypes_tblAccount");
            });

            modelBuilder.Entity<TblCustomField>(entity =>
            {
                entity.ToTable("tblCustomField");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FieldName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblCustomFields)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCustomField_tblAccount");

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.TblCustomFields)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCustomField_tblCustomFieldType");
            });

            modelBuilder.Entity<TblCustomFieldAnswer>(entity =>
            {
                entity.HasKey(e => e.CustomFieldAnsId);

                entity.ToTable("tblCustomFieldAnswer");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FieldAns).HasMaxLength(50);

                entity.Property(e => e.LeadId).HasColumnName("LeadID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblCustomFieldAnswers)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCustomFieldAnswer_tblAccount");

                entity.HasOne(d => d.CustomField)
                    .WithMany(p => p.TblCustomFieldAnswers)
                    .HasForeignKey(d => d.CustomFieldId)
                    .HasConstraintName("FK_tblCustomFieldAnswer_tblCustomField");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.TblCustomFieldAnswers)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_tblCustomFieldAnswer_tblLead");
            });

            modelBuilder.Entity<TblCustomFieldType>(entity =>
            {
                entity.ToTable("tblCustomFieldType");

                entity.Property(e => e.FieldType).HasMaxLength(50);
            });

            modelBuilder.Entity<TblCustomFieldValue>(entity =>
            {
                entity.HasKey(e => e.CustomFieldValueId);

                entity.ToTable("tblCustomFieldValue");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FieldValue).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblCustomFieldValues)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_tblCustomFieldValue_tblAccount");

                entity.HasOne(d => d.CustomField)
                    .WithMany(p => p.TblCustomFieldValues)
                    .HasForeignKey(d => d.CustomFieldId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCustomFieldValue_tblCustomField");
            });

            modelBuilder.Entity<TblEmailTemplate>(entity =>
            {
                entity.HasKey(e => e.EmailTemplateId);

                entity.ToTable("tblEmailTemplates");

                entity.Property(e => e.EmailTemplateId).HasColumnName("EmailTemplateID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailName).HasMaxLength(250);

                entity.Property(e => e.EmailSubject).HasMaxLength(500);

                entity.Property(e => e.EmailTemplateDescription).HasMaxLength(250);

                entity.Property(e => e.FromEmail).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblEmailTemplates)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblEmailTemplates_tblAccount");

                entity.HasOne(d => d.TemplateType)
                    .WithMany(p => p.TblEmailTemplates)
                    .HasForeignKey(d => d.TemplateTypeId)
                    .HasConstraintName("FK_tblEmailTemplates_tblTemplateType");
            });

            modelBuilder.Entity<TblIndustry>(entity =>
            {
                entity.HasKey(e => e.IndustryId);

                entity.ToTable("tblIndustry");

                entity.Property(e => e.IndustryId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("IndustryID");

                entity.Property(e => e.IndustryName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblLead>(entity =>
            {
                entity.HasKey(e => e.LeadId);

                entity.ToTable("tblLead");

                entity.Property(e => e.AnnualRevenue).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Company)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Industry)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LeadOwner)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LeadSource)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LeadStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SecondaryEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SkypeId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stage)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("stage");

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Street)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TwitterId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Website).HasMaxLength(500);

                entity.Property(e => e.ZipCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblLeadAccounts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblLead_tblAccount");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.TblLeadAgents)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_tblLead_tblAccount1");

                entity.HasOne(d => d.CustomField)
                    .WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.CustomFieldId)
                    .HasConstraintName("FK_tblLead_tblCustomField");

                entity.HasOne(d => d.StageNavigation)
                    .WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.StageId)
                    .HasConstraintName("FK_tblLead_tblStage");
            });

            modelBuilder.Entity<TblLeadAppointment>(entity =>
            {
                entity.HasKey(e => e.LeadAppointmentId);

                entity.ToTable("tblLeadAppointment");

                entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblLeadAppointmentAccounts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblLeadAppointment_tblAccount");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.TblLeadAppointmentAgents)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_tblLeadAppointment_tblAccount1");

                entity.HasOne(d => d.AppointmentOutcomes)
                    .WithMany(p => p.TblLeadAppointments)
                    .HasForeignKey(d => d.AppointmentOutcomesId)
                    .HasConstraintName("FK_tblLeadAppointment_tblAppointmentOutcomes");

                entity.HasOne(d => d.AppointmentType)
                    .WithMany(p => p.TblLeadAppointments)
                    .HasForeignKey(d => d.AppointmentTypeId)
                    .HasConstraintName("FK_tblLeadAppointment_tblAppointmentTypes");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.TblLeadAppointments)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_tblLeadAppointment_tblLead");
            });

            modelBuilder.Entity<TblLeadEmailMessage>(entity =>
            {
                entity.HasKey(e => e.LeadEmailMessageId);

                entity.ToTable("tblLeadEmailMessage");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailMessageId).HasDefaultValueSql("((0))");

                entity.Property(e => e.FromName).HasMaxLength(50);

                entity.Property(e => e.IsReplay).HasDefaultValueSql("((0))");

                entity.Property(e => e.ToName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblLeadEmailMessages)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblLeadEmailMessage_tblAccount");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.TblLeadEmailMessages)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_tblLeadEmailMessage_tblLead");
            });

            modelBuilder.Entity<TblLeadEmailMessageAttachment>(entity =>
            {
                entity.HasKey(e => e.LeadEmailMessageAttachmentId);

                entity.ToTable("tblLeadEmailMessageAttachment");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.LeadEmailMessage)
                    .WithMany(p => p.TblLeadEmailMessageAttachments)
                    .HasForeignKey(d => d.LeadEmailMessageId)
                    .HasConstraintName("FK_tblLeadEmailMessageAttachment_tblLeadEmailMessage");
            });

            modelBuilder.Entity<TblLeadFile>(entity =>
            {
                entity.HasKey(e => e.LeadFileId);

                entity.ToTable("tblLeadFile");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FileName).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblLeadFiles)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblLeadFile_tblAccount");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.TblLeadFiles)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_tblLeadFile_tblLead");
            });

            modelBuilder.Entity<TblLeadSource>(entity =>
            {
                entity.HasKey(e => e.LeadSourceId);

                entity.ToTable("tblLeadSource");

                entity.Property(e => e.LeadSourceId).HasColumnName("LeadSourceID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LeadSourceName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblLeadSources)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblLeadSource_tblAccount");
            });

            modelBuilder.Entity<TblLeadStatus>(entity =>
            {
                entity.HasKey(e => e.LeadStatusId);

                entity.ToTable("tblLeadStatus");

                entity.Property(e => e.LeadStatusId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LeadStatusID");

                entity.Property(e => e.LeadStatusType)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblLeadTag>(entity =>
            {
                entity.HasKey(e => e.LeadTagId);

                entity.ToTable("tblLeadTag");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblLeadTags)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblLeadTag_tblAccount");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.TblLeadTags)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_tblLeadTag_tblLead");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.TblLeadTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_tblLeadTag_tblTags");
            });

            modelBuilder.Entity<TblRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("tblRole");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblSmtp>(entity =>
            {
                entity.ToTable("tblSMTP");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblStage>(entity =>
            {
                entity.HasKey(e => e.StageId);

                entity.ToTable("tblStage");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.StageName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblStages)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblStage_tblAccount");
            });

            modelBuilder.Entity<TblTag>(entity =>
            {
                entity.HasKey(e => e.TagId);

                entity.ToTable("tblTags");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TagName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TblTags)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblTags_tblAccount");
            });

            modelBuilder.Entity<TblTemplateCategory>(entity =>
            {
                entity.HasKey(e => e.TemplateCategoryId)
                    .HasName("PK_tblTemplateCategory_1");

                entity.ToTable("tblTemplateCategory");

                entity.Property(e => e.TemplateCategoryName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblTemplateCategoryHtmlemail>(entity =>
            {
                entity.HasKey(e => e.TemplateCategoryHtmlemailId);

                entity.ToTable("tblTemplateCategoryHTMLEmail");

                entity.Property(e => e.TemplateCategoryHtmlemailId).HasColumnName("TemplateCategoryHTMLEmailID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TemplateHtmlemail).HasColumnName("TemplateHTMLEmail");

                entity.Property(e => e.TemplateHtmlemailDescription)
                    .HasMaxLength(50)
                    .HasColumnName("TemplateHTMLEmailDescription");

                entity.Property(e => e.TemplateHtmlimage)
                    .HasMaxLength(100)
                    .HasColumnName("TemplateHTMLImage");

                entity.HasOne(d => d.TemplateCategory)
                    .WithMany(p => p.TblTemplateCategoryHtmlemails)
                    .HasForeignKey(d => d.TemplateCategoryId)
                    .HasConstraintName("FK_tblTemplateCategoryHTMLEmail_tblTemplateCategory");
            });

            modelBuilder.Entity<TblTemplateType>(entity =>
            {
                entity.HasKey(e => e.TemplateTypeId)
                    .HasName("PK_tblTemplateCategory");

                entity.ToTable("tblTemplateType");

                entity.Property(e => e.TypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<TblUserLoginType>(entity =>
            {
                entity.HasKey(e => e.UserTypeId);

                entity.ToTable("tblUserLoginType");

                entity.Property(e => e.UserTypeName).HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
