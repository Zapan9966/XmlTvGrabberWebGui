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
    [Migration("20201103173542_Trace_Table")]
    partial class Trace_Table
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

                    b.Property<string>("OutputFilename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("XmlUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ConfigId");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.FiltredCategory", b =>
                {
                    b.Property<int>("FiltredCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TvHeadendCategorieId")
                        .HasColumnType("INTEGER");

                    b.HasKey("FiltredCategoryId");

                    b.HasIndex("TvHeadendCategorieId");

                    b.ToTable("FiltredCategories");
                });

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.Trace", b =>
                {
                    b.Property<int>("TraceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Level")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
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

            modelBuilder.Entity("XmlTvGrabberWebGui.Models.FiltredCategory", b =>
                {
                    b.HasOne("XmlTvGrabberWebGui.Models.TvHeadendCategory", "TvHeadendCategorie")
                        .WithMany()
                        .HasForeignKey("TvHeadendCategorieId");
                });
#pragma warning restore 612, 618
        }
    }
}