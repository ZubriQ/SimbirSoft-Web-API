using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Olymp_Project.Database
{
    public partial class AnimalChipizationContext : DbContext
    {
        public AnimalChipizationContext()
        {
        }

        public AnimalChipizationContext(DbContextOptions<AnimalChipizationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Animal> Animals { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;
        public virtual DbSet<VisitedLocation> VisitedLocations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Email).HasMaxLength(320);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(100);
            });

            modelBuilder.Entity<Animal>(entity =>
            {
                entity.ToTable("Animal");

                entity.Property(e => e.ChippingDateTime).HasColumnType("datetime");

                entity.Property(e => e.DeathDateTime).HasColumnType("datetime");

                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.LifeStatus).HasMaxLength(50);

                entity.HasOne(d => d.Chipper)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.ChipperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Animal_Account");

                entity.HasOne(d => d.ChippingLocation)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.ChippingLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Animal_Location");

                entity.HasMany(d => d.Types)
                    .WithMany(p => p.Animals)
                    .UsingEntity<Dictionary<string, object>>(
                        "AnimalType",
                        l => l.HasOne<Type>().WithMany().HasForeignKey("TypeId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_AnimalType_Type"),
                        r => r.HasOne<Animal>().WithMany().HasForeignKey("AnimalId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_AnimalType_Animal"),
                        j =>
                        {
                            j.HasKey("AnimalId", "TypeId");

                            j.ToTable("AnimalType");
                        });
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("Type");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<VisitedLocation>(entity =>
            {
                entity.ToTable("VisitedLocation");

                entity.Property(e => e.VisitDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Animal)
                    .WithMany(p => p.VisitedLocations)
                    .HasForeignKey(d => d.AnimalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VisitedLocation_Animal");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.VisitedLocations)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VisitedLocation_Location");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
