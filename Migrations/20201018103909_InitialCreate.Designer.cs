﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OKexTime.Context;

namespace OKexTime.Migrations
{
    [DbContext(typeof(OkexContext))]
    [Migration("20201018103909_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("OKexTime.Models.UsersRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("EthereumAddress")
                        .HasColumnType("character varying(42)")
                        .HasMaxLength(42);

                    b.Property<string>("EthereumStatus")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<decimal>("ExpectedAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("Phone")
                        .HasColumnType("character varying(20)")
                        .HasMaxLength(20);

                    b.Property<bool>("Settled")
                        .HasColumnType("boolean");

                    b.Property<string>("TxId")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
