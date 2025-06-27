using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestApi.Models;

public partial class TestContext : DbContext
{
    public TestContext()
    {
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblInvoice> TblInvoices { get; set; }

    public virtual DbSet<TblItem> TblItems { get; set; }

    public virtual DbSet<TblPricingList> TblPricingLists { get; set; }

    public virtual DbSet<Test1> Test1s { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-RP4DU39\\SQLEXPRESS;Database=test;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
