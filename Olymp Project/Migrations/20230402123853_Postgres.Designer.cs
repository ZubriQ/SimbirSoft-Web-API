﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;
using Olymp_Project.Models;

#nullable disable

namespace Olymp_Project.Migrations
{
    [DbContext(typeof(ChipizationDbContext))]
    [Migration("20230402123853_Postgres")]
    partial class Postgres
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AnimalKind", b =>
                {
                    b.Property<long>("AnimalsId")
                        .HasColumnType("bigint");

                    b.Property<long>("KindsId")
                        .HasColumnType("bigint");

                    b.HasKey("AnimalsId", "KindsId");

                    b.HasIndex("KindsId");

                    b.ToTable("AnimalKind");
                });

            modelBuilder.Entity("Olymp_Project.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@simbirsoft.com",
                            FirstName = "adminFirstName",
                            LastName = "adminLastName",
                            Password = "qwerty123",
                            Role = "ADMIN"
                        },
                        new
                        {
                            Id = 2,
                            Email = "chipper@simbirsoft.com",
                            FirstName = "chipperFirstName",
                            LastName = "chipperLastName",
                            Password = "qwerty123",
                            Role = "CHIPPER"
                        },
                        new
                        {
                            Id = 3,
                            Email = "user@simbirsoft.com",
                            FirstName = "userFirstName",
                            LastName = "userLastName",
                            Password = "qwerty123",
                            Role = "USER"
                        });
                });

            modelBuilder.Entity("Olymp_Project.Models.Animal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ChipperId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ChippingDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ChippingLocationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("DeathDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<float>("Length")
                        .HasColumnType("real");

                    b.Property<string>("LifeStatus")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ChipperId");

                    b.HasIndex("ChippingLocationId");

                    b.ToTable("Animals");
                });

            modelBuilder.Entity("Olymp_Project.Models.Area", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<NpgsqlPolygon>("Points")
                        .HasColumnType("polygon");

                    b.HasKey("Id");

                    b.ToTable("Areas");
                });

            modelBuilder.Entity("Olymp_Project.Models.Kind", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Kinds");
                });

            modelBuilder.Entity("Olymp_Project.Models.Location", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Olymp_Project.Models.VisitedLocation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AnimalId")
                        .HasColumnType("bigint");

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("VisitDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId");

                    b.HasIndex("LocationId");

                    b.ToTable("VisitedLocations");
                });

            modelBuilder.Entity("AnimalKind", b =>
                {
                    b.HasOne("Olymp_Project.Models.Animal", null)
                        .WithMany()
                        .HasForeignKey("AnimalsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Olymp_Project.Models.Kind", null)
                        .WithMany()
                        .HasForeignKey("KindsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Olymp_Project.Models.Animal", b =>
                {
                    b.HasOne("Olymp_Project.Models.Account", "Chipper")
                        .WithMany("Animals")
                        .HasForeignKey("ChipperId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Olymp_Project.Models.Location", "ChippingLocation")
                        .WithMany("Animals")
                        .HasForeignKey("ChippingLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chipper");

                    b.Navigation("ChippingLocation");
                });

            modelBuilder.Entity("Olymp_Project.Models.VisitedLocation", b =>
                {
                    b.HasOne("Olymp_Project.Models.Animal", "Animal")
                        .WithMany("VisitedLocations")
                        .HasForeignKey("AnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Olymp_Project.Models.Location", "Location")
                        .WithMany("VisitedLocations")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Animal");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Olymp_Project.Models.Account", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("Olymp_Project.Models.Animal", b =>
                {
                    b.Navigation("VisitedLocations");
                });

            modelBuilder.Entity("Olymp_Project.Models.Location", b =>
                {
                    b.Navigation("Animals");

                    b.Navigation("VisitedLocations");
                });
#pragma warning restore 612, 618
        }
    }
}