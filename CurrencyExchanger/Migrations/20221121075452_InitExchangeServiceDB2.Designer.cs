﻿// <auto-generated />
using System;
using CurrencyExchanger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CurrencyExchanger.Migrations
{
    [DbContext(typeof(ExchangeServiceDBContext))]
    [Migration("20221121075452_InitExchangeServiceDB2")]
    partial class InitExchangeServiceDB2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CurrencyExchanger.Models.Database.Exchange", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(3)");

                    b.Property<decimal>("FromAmount")
                        .HasColumnType("decimal(20,7)");

                    b.Property<bool>("Historical")
                        .HasColumnType("bit");

                    b.Property<DateTime>("PerformedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Succeded")
                        .HasColumnType("bit");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("nvarchar(3)");

                    b.Property<decimal>("ToAmount")
                        .HasColumnType("decimal(20,7)");

                    b.HasKey("Id");

                    b.ToTable("exchanges");
                });

            modelBuilder.Entity("CurrencyExchanger.Models.Database.ExchangeRate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(3)");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(20,7)");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("nvarchar(3)");

                    b.HasKey("Id");

                    b.ToTable("rates");
                });
#pragma warning restore 612, 618
        }
    }
}
