using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DBLayer.Models;

public partial class TestContext : DbContext
{
    public TestContext()
    {
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBranch> TblBranches { get; set; }

    public virtual DbSet<TblCompany> TblCompanies { get; set; }

    public virtual DbSet<TblCompanyUnit> TblCompanyUnits { get; set; }

    public virtual DbSet<TblDepartment> TblDepartments { get; set; }

    public virtual DbSet<TblDivision> TblDivisions { get; set; }

    public virtual DbSet<TblInvoice> TblInvoices { get; set; }

    public virtual DbSet<TblItem> TblItems { get; set; }

    public virtual DbSet<TblOrganisation> TblOrganisations { get; set; }

    public virtual DbSet<TblPricingList> TblPricingLists { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<Test1> Test1s { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-RP4DU39\\SQLEXPRESS;Database=Test;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBranch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__tblBranc__A1682FA549DC0CD4");

            entity.ToTable("tblBranch");

            entity.Property(e => e.BranchId)
                .HasDefaultValue(-1L)
                .HasColumnName("BranchID");
            entity.Property(e => e.BranchRefNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.BranchTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TblCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__tblCompa__2D971C4C49A740A2");

            entity.ToTable("tblCompany");

            entity.Property(e => e.CompanyId)
                .HasDefaultValue(-1L)
                .HasColumnName("CompanyID");
            entity.Property(e => e.CompanyRefNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CompanyTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TblCompanyUnit>(entity =>
        {
            entity.HasKey(e => e.CompanyUnitId).HasName("PK__tblCompa__8942C15160273FA8");

            entity.ToTable("tblCompanyUnit");

            entity.Property(e => e.CompanyUnitId)
                .HasDefaultValue(-1L)
                .HasColumnName("CompanyUnitID");
            entity.Property(e => e.CompanyUnitRefNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CompanyUnitTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TblDepartment>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__tblDepar__B2079BCD048EDF58");

            entity.ToTable("tblDepartment");

            entity.Property(e => e.DepartmentId)
                .HasDefaultValue(-1L)
                .HasColumnName("DepartmentID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentRefNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DepartmentTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<TblDivision>(entity =>
        {
            entity.HasKey(e => e.DivisionId).HasName("PK__tblDivis__20EFC6880BD734EE");

            entity.ToTable("tblDivision");

            entity.Property(e => e.DivisionId)
                .HasDefaultValue(-1L)
                .HasColumnName("DivisionID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DivisionRefNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DivisionTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<TblInvoice>(entity =>
        {
            entity.HasKey(e => e.TranId).HasName("PK__tblInvoi__F7089629B6D53D4A");

            entity.ToTable("tblInvoice");

            entity.Property(e => e.TranId)
                .HasDefaultValue(-1L)
                .HasColumnName("TranID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.PricingListId).HasColumnName("PricingListID");
            entity.Property(e => e.TranRefNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<TblItem>(entity =>
        {
            entity.HasKey(e => e.TranId).HasName("PK__tblItem__F7089629020F2AD7");

            entity.ToTable("tblItem");

            entity.Property(e => e.TranId)
                .ValueGeneratedNever()
                .HasColumnName("TranID");
            entity.Property(e => e.ItemRefNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.ItemTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SaleRate).HasDefaultValue(0.0);
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TblOrganisation>(entity =>
        {
            entity.HasKey(e => e.OrganisationId).HasName("PK__tblOrgan__722346BCA4BE2335");

            entity.ToTable("tblOrganisation");

            entity.Property(e => e.OrganisationId)
                .HasDefaultValue(-1L)
                .HasColumnName("OrganisationID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrganisationRefNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.OrganisationTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<TblPricingList>(entity =>
        {
            entity.HasKey(e => e.TranId).HasName("PK__tblPrici__F7089629E1C238BE");

            entity.ToTable("tblPricingList");

            entity.Property(e => e.TranId)
                .ValueGeneratedNever()
                .HasColumnName("TranID");
            entity.Property(e => e.EffictiveFrom).HasColumnType("datetime");
            entity.Property(e => e.EffictiveTo).HasColumnType("datetime");
            entity.Property(e => e.ItemId)
                .HasDefaultValue(0)
                .HasColumnName("ItemID");
            entity.Property(e => e.PricingTitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SaleRate).HasDefaultValue(0.0);
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PK__tblUsers__1797D02499DA0056");

            entity.ToTable("tblUsers");

            entity.Property(e => e.Userid).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(233)
                .IsUnicode(false);
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Test1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__test1__3213E83FE4C48E12");

            entity.ToTable("test1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
