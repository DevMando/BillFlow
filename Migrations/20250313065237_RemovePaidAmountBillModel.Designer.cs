﻿// <auto-generated />
using System;
using BillFlow.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BillFlow.Migrations
{
    [DbContext(typeof(BillFlowContext))]
    [Migration("20250313065237_RemovePaidAmountBillModel")]
    partial class RemovePaidAmountBillModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BillFlow.Models.Bill", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal>("AmountDue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("BillTagID")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsMonthy")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPayedOff")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("BillTagID");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("BillFlow.Models.BillTag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("BillTags");
                });

            modelBuilder.Entity("BillFlow.Models.Payment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BillID")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("BillID");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("BillFlow.Models.Bill", b =>
                {
                    b.HasOne("BillFlow.Models.BillTag", "BillTag")
                        .WithMany()
                        .HasForeignKey("BillTagID");

                    b.Navigation("BillTag");
                });

            modelBuilder.Entity("BillFlow.Models.Payment", b =>
                {
                    b.HasOne("BillFlow.Models.Bill", "Bill")
                        .WithMany("Payments")
                        .HasForeignKey("BillID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bill");
                });

            modelBuilder.Entity("BillFlow.Models.Bill", b =>
                {
                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
