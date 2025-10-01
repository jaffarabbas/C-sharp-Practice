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

    public virtual DbSet<TblActionType> TblActionTypes { get; set; }

    public virtual DbSet<TblBranch> TblBranches { get; set; }

    public virtual DbSet<TblCompany> TblCompanies { get; set; }

    public virtual DbSet<TblCompanyUnit> TblCompanyUnits { get; set; }

    public virtual DbSet<TblDepartment> TblDepartments { get; set; }

    public virtual DbSet<TblDivision> TblDivisions { get; set; }

    public virtual DbSet<TblInvoice> TblInvoices { get; set; }

    public virtual DbSet<TblItem> TblItems { get; set; }

    public virtual DbSet<TblOrganisation> TblOrganisations { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblPricingList> TblPricingLists { get; set; }

    public virtual DbSet<TblResource> TblResources { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblRolePermission> TblRolePermissions { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserRole> TblUserRoles { get; set; }

    public virtual DbSet<TblResetToken> TblResetTokens { get; set; }

    public virtual DbSet<TblTokenType> TblTokenTypes { get; set; }

    public virtual DbSet<Test1> Test1s { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblActionType>(entity =>
        {
            entity.HasKey(e => e.ActionTypeId).HasName("PK__tblActio__62FE4C04B5ED9D4D");

            entity.ToTable("tblActionType");

            entity.Property(e => e.ActionTypeId)
                .ValueGeneratedNever()
                .HasColumnName("ActionTypeID");
            entity.Property(e => e.ActionTypeCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ActionTypeIsActive).HasDefaultValue(true);
            entity.Property(e => e.ActionTypeTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

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

        modelBuilder.Entity<TblPermission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__tblPermi__EFA6FB0F4AD1F430");

            entity.ToTable("tblPermission");

            entity.Property(e => e.PermissionId)
                .ValueGeneratedNever()
                .HasColumnName("PermissionID");
            entity.Property(e => e.ActionTypeId).HasColumnName("ActionTypeID");
            entity.Property(e => e.PermissionCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PermissionIsActive).HasDefaultValue(true);
            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");

            entity.HasOne(d => d.ActionType).WithMany(p => p.TblPermissions)
                .HasForeignKey(d => d.ActionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permission_Action");

            entity.HasOne(d => d.Resource).WithMany(p => p.TblPermissions)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permission_Resource");
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

        modelBuilder.Entity<TblResource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__tblResou__4ED1814FFC50535C");

            entity.ToTable("tblResource");

            entity.Property(e => e.ResourceId)
                .ValueGeneratedNever()
                .HasColumnName("ResourceID");
            entity.Property(e => e.ResourceCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ResourceIsActive).HasDefaultValue(true);
            entity.Property(e => e.ResourceName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tblRole__8AFACE3A3A043175");

            entity.ToTable("tblRole");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleIsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("");

            entity.HasMany(d => d.ChildRoles).WithMany(p => p.ParentRoles)
                .UsingEntity<Dictionary<string, object>>(
                    "TblRoleHierarchy",
                    r => r.HasOne<TblRole>().WithMany()
                        .HasForeignKey("ChildRoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RoleHierarchy_Child"),
                    l => l.HasOne<TblRole>().WithMany()
                        .HasForeignKey("ParentRoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RoleHierarchy_Parent"),
                    j =>
                    {
                        j.HasKey("ParentRoleId", "ChildRoleId").HasName("PK_RoleHierarchy");
                        j.ToTable("tblRoleHierarchy");
                        j.IndexerProperty<int>("ParentRoleId").HasColumnName("ParentRoleID");
                        j.IndexerProperty<int>("ChildRoleId").HasColumnName("ChildRoleID");
                    });

            entity.HasMany(d => d.ParentRoles).WithMany(p => p.ChildRoles)
                .UsingEntity<Dictionary<string, object>>(
                    "TblRoleHierarchy",
                    r => r.HasOne<TblRole>().WithMany()
                        .HasForeignKey("ParentRoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RoleHierarchy_Parent"),
                    l => l.HasOne<TblRole>().WithMany()
                        .HasForeignKey("ChildRoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RoleHierarchy_Child"),
                    j =>
                    {
                        j.HasKey("ParentRoleId", "ChildRoleId").HasName("PK_RoleHierarchy");
                        j.ToTable("tblRoleHierarchy");
                        j.IndexerProperty<int>("ParentRoleId").HasColumnName("ParentRoleID");
                        j.IndexerProperty<int>("ChildRoleId").HasColumnName("ChildRoleID");
                    });
        });

        modelBuilder.Entity<TblRolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK__tblRoleP__120F469A84A97AB2");

            entity.ToTable("tblRolePermission");

            entity.Property(e => e.RolePermissionId)
                .ValueGeneratedNever()
                .HasColumnName("RolePermissionID");
            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RolePermissionCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RolePermissionIsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Permission).WithMany(p => p.TblRolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermission_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.TblRolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermission_Role");
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

        modelBuilder.Entity<TblResetToken>(entity =>
        {
            entity.HasKey(e => e.ResetTokenId);

            entity.ToTable("tblResetToken");

            entity.Property(e => e.ResetTokenId).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.TokenType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResetToken_User");

            entity.HasOne(d => d.TokenTypeNavigation).WithMany(p => p.TblResetTokens)
                .HasForeignKey(d => d.TokenType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResetToken_TokenType");
        });

        modelBuilder.Entity<TblTokenType>(entity =>
        {
            entity.HasKey(e => e.TokenType);

            entity.ToTable("tblTokenType");

            entity.Property(e => e.TokenType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblUserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__tblUserR__3D978A55AEDF4DB3");

            entity.ToTable("tblUserRole");

            entity.Property(e => e.UserRoleId)
                .ValueGeneratedNever()
                .HasColumnName("UserRoleID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserRoleCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserRoleIsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.TblUserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_User");
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
