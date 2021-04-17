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

        public virtual DbSet<TblAgent> TblAgents { get; set; }
        public virtual DbSet<TblCompany> TblCompanies { get; set; }
        public virtual DbSet<TblIndustry> TblIndustries { get; set; }
        public virtual DbSet<TblLead> TblLeads { get; set; }
        public virtual DbSet<TblLeadSource> TblLeadSources { get; set; }
        public virtual DbSet<TblLeadStatus> TblLeadStatuses { get; set; }
        public virtual DbSet<TblLeadTag> TblLeadTags { get; set; }
        public virtual DbSet<TblStage> TblStages { get; set; }
        public virtual DbSet<TblTag> TblTags { get; set; }
        public virtual DbSet<TblUser> TblUsers { get; set; }
        public virtual DbSet<TblUserLoginType> TblUserLoginTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=DESKTOP-VC6KLNP\\SQLEXPRESSSANJU;user id=sa;password=Potenza@123;Database=RealEstate;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer(@"Server=Estajo\SQLEXPRESS;user id=sa;password=estajo@123;Database=dbEstajo;Trusted_Connection=False;MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TblAgent>(entity =>
            {
                entity.ToTable("tblAgent");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CellPhone).HasMaxLength(20);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(50);

                entity.Property(e => e.FullName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblAgents)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblAgent_tblCompany");

                entity.HasOne(d => d.UserLoginType)
                    .WithMany(p => p.TblAgents)
                    .HasForeignKey(d => d.UserLoginTypeId)
                    .HasConstraintName("FK_tblAgent_tblUserLoginType");
            });

            modelBuilder.Entity<TblCompany>(entity =>
            {
                entity.HasKey(e => e.CompanyId);

                entity.ToTable("tblCompany");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CellPhone).HasMaxLength(20);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.State).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.ZipCode).HasMaxLength(50);

                entity.HasOne(d => d.LogionType)
                    .WithMany(p => p.TblCompanies)
                    .HasForeignKey(d => d.LogionTypeId)
                    .HasConstraintName("FK_tblCompany_tblUserLoginType");
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

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_tblLead_tblAgent");

                entity.HasOne(d => d.CompanyNavigation)
                    .WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblLead_tblCompany");

                entity.HasOne(d => d.StageNavigation)
                    .WithMany(p => p.TblLeads)
                    .HasForeignKey(d => d.StageId)
                    .HasConstraintName("FK_tblLead_tblStage");
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

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblLeadSources)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblLeadSource_tblCompany");
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

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblLeadTags)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblLeadTag_tblCompany");

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.TblLeadTags)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_tblLeadTag_tblLead");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.TblLeadTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_tblLeadTag_tblTags");
            });

            modelBuilder.Entity<TblStage>(entity =>
            {
                entity.HasKey(e => e.StageId);

                entity.ToTable("tblStage");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.StageName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblStages)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblStage_tblCompany");
            });

            modelBuilder.Entity<TblTag>(entity =>
            {
                entity.HasKey(e => e.TagId);

                entity.ToTable("tblTags");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TagName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblTags)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblTags_tblCompany");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("tblUser");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblUsers)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblUser_tblCompany");

                entity.HasOne(d => d.UserLoginType)
                    .WithMany(p => p.TblUsers)
                    .HasForeignKey(d => d.UserLoginTypeId)
                    .HasConstraintName("FK_tblUser_tblUserLoginType");
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
