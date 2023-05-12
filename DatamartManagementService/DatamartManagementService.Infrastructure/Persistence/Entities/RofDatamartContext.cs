using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.Entities
{
    public partial class RofDatamartContext : DbContext
    {
        public RofDatamartContext()
        {
        }

        public RofDatamartContext(DbContextOptions<RofDatamartContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EmployeePayroll> EmployeePayroll { get; set; }
        public virtual DbSet<EmployeePayrollDetail> EmployeePayrollDetail { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-R3ND13SE\\SQLEXPRESS;Database=RofDatamart;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeePayroll>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.EmployeeTotalPay).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.PayPeriodEndDate).HasColumnType("date");

                entity.Property(e => e.PayPeriodStartDate).HasColumnType("date");
            });

            modelBuilder.Entity<EmployeePayrollDetail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.EmployeePayForService).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.PetServiceName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceDurationTimeUnit)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceEndDateTime).HasColumnType("datetime");

                entity.Property(e => e.ServiceStartDateTime).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
