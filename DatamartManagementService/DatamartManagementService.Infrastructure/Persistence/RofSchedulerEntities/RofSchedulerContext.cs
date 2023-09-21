using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities
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

        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<HolidayRates> HolidayRates { get; set; }
        public virtual DbSet<Holidays> Holidays { get; set; }
        public virtual DbSet<JobEvent> JobEvent { get; set; }
        public virtual DbSet<PetServices> PetServices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-R3ND13SE\\SQLEXPRESS;Database=RofScheduler;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.Ssn)
                    .HasName("UC_SSN")
                    .IsUnique();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.AddressLine1)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Ssn)
                    .IsRequired()
                    .HasColumnName("SSN")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

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
                    .HasConstraintName("FK__HolidayRa__Holid__5F691F13");

                entity.HasOne(d => d.PetService)
                    .WithMany(p => p.HolidayRates)
                    .HasForeignKey(d => d.PetServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HolidayRa__PetSe__5E74FADA");
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

            modelBuilder.Entity<JobEvent>(entity =>
            {
                entity.HasIndex(e => new { e.EmployeeId, e.PetId, e.EventStartTime, e.EventEndTime })
                    .HasName("UC_EVENT")
                    .IsUnique();

                entity.Property(e => e.EventEndTime).HasColumnType("datetime");

                entity.Property(e => e.EventStartTime).HasColumnType("datetime");

                entity.Property(e => e.LastModifiedDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.JobEvent)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__JobEvent__Employ__7928F116");

                entity.HasOne(d => d.PetService)
                    .WithMany(p => p.JobEvent)
                    .HasForeignKey(d => d.PetServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__JobEvent__PetSer__7B113988");
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
