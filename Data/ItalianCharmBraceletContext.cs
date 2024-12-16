using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ItalianCharmBracelet.Data;

public partial class ItalianCharmBraceletContext : DbContext
{
    public ItalianCharmBraceletContext()
    {
    }

    public ItalianCharmBraceletContext(DbContextOptions<ItalianCharmBraceletContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Charm> Charms { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

    public virtual DbSet<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; }

    public virtual DbSet<SalesInvoice> SalesInvoices { get; set; }

    public virtual DbSet<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("CATEGORY");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Note)
                .HasColumnType("ntext")
                .HasColumnName("NOTE");
        });

        modelBuilder.Entity<Charm>(entity =>
        {
            entity.ToTable("CHARM");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.CateId)
                .HasMaxLength(50)
                .HasColumnName("CATE_ID");
            entity.Property(e => e.Description)
                .HasColumnType("ntext")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Discount).HasColumnName("DISCOUNT");
            entity.Property(e => e.Img)
                .HasMaxLength(50)
                .HasColumnName("IMG");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STATE");
            entity.Property(e => e.Tag)
                .HasMaxLength(50)
                .HasColumnName("TAG");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("UNIT");

            entity.HasOne(d => d.Cate).WithMany(p => p.Charms)
                .HasForeignKey(d => d.CateId)
                .HasConstraintName("FK_CHARM_CATEGORY");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("CUSTOMER");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Cell)
                .HasMaxLength(50)
                .HasColumnName("CELL");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("FIRST_NAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("GENDER");
            entity.Property(e => e.Img)
                .HasMaxLength(50)
                .HasColumnName("IMG");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("LAST_NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.RandomKey)
                .HasMaxLength(50)
                .HasColumnName("RANDOM_KEY");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("ROLE");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("STATE");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<PurchaseInvoice>(entity =>
        {
            entity.ToTable("PURCHASE_INVOICE");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnName("DATE");
            entity.Property(e => e.Note)
                .HasColumnType("ntext")
                .HasColumnName("NOTE");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("STATE");
            entity.Property(e => e.TotalPayment).HasColumnName("TOTAL_PAYMENT");
        });

        modelBuilder.Entity<PurchaseInvoiceDetail>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ProductId });

            entity.ToTable("PURCHASE_INVOICE_DETAIL");

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .HasColumnName("INVOICE_ID");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .HasColumnName("PRODUCT_ID");
            entity.Property(e => e.Link)
                .HasMaxLength(2083)
                .IsUnicode(false)
                .HasColumnName("LINK");
            entity.Property(e => e.Note)
                .HasColumnType("ntext")
                .HasColumnName("NOTE");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Invoice).WithMany(p => p.PurchaseInvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PURCHASE_INVOICE_DETAIL_PURCHASE_INVOICE");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseInvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PURCHASE_INVOICE_DETAIL_CHARM");
        });

        modelBuilder.Entity<SalesInvoice>(entity =>
        {
            entity.ToTable("SALES_INVOICE");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Cell)
                .HasMaxLength(50)
                .HasColumnName("CELL");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasColumnName("CUSTOMER_ID");
            entity.Property(e => e.Date).HasColumnName("DATE");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Note)
                .HasColumnType("ntext")
                .HasColumnName("NOTE");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("PAYMENT_METHOD");
            entity.Property(e => e.StateId)
                .HasMaxLength(50)
                .HasColumnName("STATE_ID");
            entity.Property(e => e.TotalPayment).HasColumnName("TOTAL_PAYMENT");
            entity.Property(e => e.VoucherId)
                .HasMaxLength(50)
                .HasColumnName("VOUCHER_ID");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesInvoices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_SALES_INVOICE_CUSTOMER");

            entity.HasOne(d => d.State).WithMany(p => p.SalesInvoices)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK_SALES_INVOICE_STATE");

            entity.HasOne(d => d.Voucher).WithMany(p => p.SalesInvoices)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK_SALES_INVOICE_VOUCHER");
        });

        modelBuilder.Entity<SalesInvoiceDetail>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ProductId });

            entity.ToTable("SALES_INVOICE_DETAIL");

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .HasColumnName("INVOICE_ID");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .HasColumnName("PRODUCT_ID");
            entity.Property(e => e.Note)
                .HasColumnType("ntext")
                .HasColumnName("NOTE");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Invoice).WithMany(p => p.SalesInvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SALES_INVOICE_DETAIL_SALES_INVOICE");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesInvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SALES_INVOICE_DETAIL_CHARM");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("STATE");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.ToTable("VOUCHER");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("CODE");
            entity.Property(e => e.DiscountAmount).HasColumnName("DISCOUNT_AMOUNT");
            entity.Property(e => e.Discription)
                .HasColumnType("ntext")
                .HasColumnName("DISCRIPTION");
            entity.Property(e => e.EndDate).HasColumnName("END_DATE");
            entity.Property(e => e.MaxDiscount).HasColumnName("MAX_DISCOUNT");
            entity.Property(e => e.MinInvoiceValue).HasColumnName("MIN_INVOICE_VALUE");
            entity.Property(e => e.Note)
                .HasColumnType("ntext")
                .HasColumnName("NOTE");
            entity.Property(e => e.PercentDiscount).HasColumnName("PERCENT_DISCOUNT");
            entity.Property(e => e.StartDate).HasColumnName("START_DATE");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STATE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
