﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Olymp_Project.Models
{
    public partial class ChipizationDbContext : DbContext
    {
        public ChipizationDbContext()
        {
        }

        public ChipizationDbContext(DbContextOptions<ChipizationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Animal> Animals { get; set; } = null!;
        public virtual DbSet<Kind> Kinds { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<VisitedLocation> VisitedLocations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Email).HasMaxLength(320);

                entity.Property(e => e.FirstName).HasMaxLength(150);

                entity.Property(e => e.LastName).HasMaxLength(150);

                entity.Property(e => e.Password).HasMaxLength(150);
            });

            modelBuilder.Entity<Animal>(entity =>
            {
                entity.ToTable("Animal");

                entity.HasIndex(e => e.ChipperId, "IX_Animal_ChipperId");

                entity.HasIndex(e => e.ChippingLocationId, "IX_Animal_ChippingLocationId");

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

                entity.HasMany(d => d.Kinds)
                    .WithMany(p => p.Animals)
                    .UsingEntity<Dictionary<string, object>>(
                        "AnimalKind",
                        l => l.HasOne<Kind>().WithMany().HasForeignKey("KindId").OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_AnimalType_Type"),
                        r => r.HasOne<Animal>().WithMany().HasForeignKey("AnimalId").OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_AnimalType_Animal"),
                        j =>
                        {
                            j.HasKey("AnimalId", "KindId").HasName("PK_AnimalType");

                            j.ToTable("AnimalKind");

                            j.HasIndex(new[] { "KindId" }, "IX_AnimalKind_KindId");
                        });
            });

            modelBuilder.Entity<Kind>(entity =>
            {
                entity.ToTable("Kind");

                entity.Property(e => e.Name).HasMaxLength(150);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");
            });

            modelBuilder.Entity<VisitedLocation>(entity =>
            {
                entity.ToTable("VisitedLocation");

                entity.HasIndex(e => e.AnimalId, "IX_VisitedLocation_AnimalId");

                entity.HasIndex(e => e.LocationId, "IX_VisitedLocation_LocationId");

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
