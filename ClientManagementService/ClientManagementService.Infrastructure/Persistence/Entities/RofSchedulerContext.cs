using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
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

        public virtual DbSet<Breed> Breeds { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Pet> Pets { get; set; }
        public virtual DbSet<PetToVaccine> PetToVaccines { get; set; }
        public virtual DbSet<PetType> PetTypes { get; set; }
        public virtual DbSet<Vaccine> Vaccines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-R3ND13SE\\SQLEXPRESS;Database=RofScheduler;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Breed>(entity =>
            {
                entity.ToTable("Breed");

                entity.Property(e => e.BreedName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PetType)
                    .WithMany(p => p.Breeds)
                    .HasForeignKey(d => d.PetTypeId)
                    .HasConstraintName("FK__Breed__PetTypeId__114A936A");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Client");

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
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.PrimaryPhoneNum)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.SecondaryPhoneNum)
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

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Client__CountryI__76969D2E");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Pet>(entity =>
            {
                entity.ToTable("Pet");

                entity.HasIndex(e => new { e.OwnerId, e.Name }, "UC_PET")
                    .IsUnique();

                entity.Property(e => e.Dob)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("DOB");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OtherInfo)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.Weight).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Breed)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.BreedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Pet__BreedId__17036CC0");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Pet__OwnerId__151B244E");

                entity.HasOne(d => d.PetType)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.PetTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Pet__PetTypeId__160F4887");
            });

            modelBuilder.Entity<PetToVaccine>(entity =>
            {
                entity.ToTable("PetToVaccine");

                entity.Property(e => e.Inoculated)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.HasOne(d => d.Pet)
                    .WithMany(p => p.PetToVaccines)
                    .HasForeignKey(d => d.PetId)
                    .HasConstraintName("FK__PetToVacc__PetId__1EA48E88");

                entity.HasOne(d => d.Vax)
                    .WithMany(p => p.PetToVaccines)
                    .HasForeignKey(d => d.VaxId)
                    .HasConstraintName("FK__PetToVacc__VaxId__1F98B2C1");
            });

            modelBuilder.Entity<PetType>(entity =>
            {
                entity.ToTable("PetType");

                entity.Property(e => e.PetTypeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Vaccine>(entity =>
            {
                entity.Property(e => e.VaxName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PetType)
                    .WithMany(p => p.Vaccines)
                    .HasForeignKey(d => d.PetTypeId)
                    .HasConstraintName("FK__Vaccines__PetTyp__7F2BE32F");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
