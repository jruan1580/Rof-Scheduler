using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PetServiceManagement.Infrastructure.Persistence.Entities
{
    public partial class RofSchedulerContext : DbContext
    {
        public RofSchedulerContext()
        {
        }

        public RofSchedulerContext(DbContextOptions<RofSchedulerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HolidayRates> HolidayRates { get; set; }
        public virtual DbSet<Holidays> Holidays { get; set; }
        public virtual DbSet<PetServices> PetServices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-ES17IBF4;Database=RofScheduler;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HolidayRates>(entity =>
            {
                entity.HasIndex(e => new { e.PetServiceId, e.HolidayId })
                    .HasName("UC_PetServiceId_HolidayId")
                    .IsUnique();

                entity.Property(e => e.HolidayRate).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.Holiday)
                    .WithMany(p => p.HolidayRates)
                    .HasForeignKey(d => d.HolidayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HolidayRa__Holid__01D345B0");

                entity.HasOne(d => d.PetService)
                    .WithMany(p => p.HolidayRates)
                    .HasForeignKey(d => d.PetServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HolidayRa__PetSe__00DF2177");
            });

            modelBuilder.Entity<Holidays>(entity =>
            {
                entity.HasIndex(e => e.HolidayName)
                    .HasName("UC_HolidayName")
                    .IsUnique();

                entity.Property(e => e.HolidayName)
                    .IsRequired()
                    .HasMaxLength(55)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PetServices>(entity =>
            {
                entity.HasIndex(e => e.ServiceName)
                    .HasName("UC_ServiceName")
                    .IsUnique();

                entity.Property(e => e.Description)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.EmployeeRate).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TimeUnit)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
