﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XmlTvGrabberWebGui.Data;

namespace XmlTvGrabberWebGui.Migrations
{
    [DbContext(typeof(GrabberContext))]
    [Migration("20201112201344_XmlUrlsTable")]
    partial class XmlUrlsTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9");

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.Config", b =>
                {
                    b.Property<int>("ConfigId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EpgDatabasePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OutputFilename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SockPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ConfigId");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.Trace", b =>
                {
                    b.Property<int>("TraceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("EventId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LogLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TraceId");

                    b.ToTable("Traces");
                });

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.TvHeadendCategory", b =>
                {
                    b.Property<int>("TvHeadendCategorieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TvHeadendCategorieId");

                    b.ToTable("TvHeadendCategories");
                });

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.XmlCategory", b =>
                {
                    b.Property<int>("XmlCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TvHeadendCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("XmlCategoryId");

                    b.HasIndex("TvHeadendCategoryId");

                    b.ToTable("XmlCategories");
                });

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.XmlUrl", b =>
                {
                    b.Property<int>("XmlUrlId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("XmlUrlId");

                    b.ToTable("XmlUrls");
                });

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.XmlCategory", b =>
                {
                    b.HasOne("XmlTvGrabberWebGui.Models.TvHeadendCategory", "TvHeadendCategory")
                        .WithMany("XmlCategories")
                        .HasForeignKey("TvHeadendCategoryId");
                });
#pragma warning restore 612, 618
        }
    }
}
