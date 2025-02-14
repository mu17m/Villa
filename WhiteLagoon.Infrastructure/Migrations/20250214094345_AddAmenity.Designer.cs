﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WhiteLagoon.Infrastructure.Data;

#nullable disable

namespace WhiteLagoon.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250214094345_AddAmenity")]
    partial class AddAmenity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.Amenity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VillaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VillaId");

                    b.ToTable("Amenities");
                });

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.Villa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("Created_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Occupancy")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Sqft")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Updated_Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Villas");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                            ImageUrl = "https://placehold.co/600x400",
                            Name = "Royal Villa",
                            Occupancy = 4,
                            Price = 200m,
                            Sqft = 550
                        },
                        new
                        {
                            Id = 2,
                            Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                            ImageUrl = "https://placehold.co/600x401",
                            Name = "Premium Pool Villa",
                            Occupancy = 4,
                            Price = 300m,
                            Sqft = 550
                        },
                        new
                        {
                            Id = 3,
                            Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                            ImageUrl = "https://placehold.co/600x402",
                            Name = "Luxury Pool Villa",
                            Occupancy = 4,
                            Price = 400m,
                            Sqft = 750
                        });
                });

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.VillaNumber", b =>
                {
                    b.Property<int>("Villa_Number")
                        .HasColumnType("int");

                    b.Property<string>("SpecialDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VillaId")
                        .HasColumnType("int");

                    b.HasKey("Villa_Number");

                    b.HasIndex("VillaId");

                    b.ToTable("VillaNumbers");

                    b.HasData(
                        new
                        {
                            Villa_Number = 101,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 102,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 103,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 104,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 201,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 202,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 203,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 301,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 302,
                            VillaId = 1
                        });
                });

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.Amenity", b =>
                {
                    b.HasOne("WhiteLagoon.Domain.Entities.Villa", "Villa")
                        .WithMany()
                        .HasForeignKey("VillaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Villa");
                });

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.VillaNumber", b =>
                {
                    b.HasOne("WhiteLagoon.Domain.Entities.Villa", "villa")
                        .WithMany()
                        .HasForeignKey("VillaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("villa");
                });
#pragma warning restore 612, 618
        }
    }
}
