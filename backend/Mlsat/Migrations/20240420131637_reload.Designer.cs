﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mlsat.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mlsat.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240420131637_reload")]
    partial class reload
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Mlsat.Models.Entities.Columns.Column", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("DataSourceId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DataSourceId");

                    b.ToTable("Column");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.DataSources.DataSource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BaseDataSourceId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDstLoaded")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLoadAp")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLoadDst")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLoadKp")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLoadWolf")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsNaDropped")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsNormalize")
                        .HasColumnType("boolean");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("TimeColumn")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BaseDataSourceId");

                    b.HasIndex("ProjectId");

                    b.ToTable("DataSources");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Models.BaseModels.Model", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DataSourceId")
                        .HasColumnType("integer");

                    b.Property<int>("ModelType")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DataSourceId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Models");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Models.ModelColumn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ModelId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.ToTable("ModelColumn");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Models.SpaceWeatherColumn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ModelId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.ToTable("SpaceWeatherColumn");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Projects.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.SpaceWeather.Ap", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal?>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Date");

                    b.ToTable("Aps");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.SpaceWeather.Dst", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal?>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Date");

                    b.ToTable("Dsts");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.SpaceWeather.Kp", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal?>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Date");

                    b.ToTable("Kps");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.SpaceWeather.Wolf", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal?>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Date");

                    b.ToTable("Wolfs");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Columns.Column", b =>
                {
                    b.HasOne("Mlsat.Models.Entities.DataSources.DataSource", null)
                        .WithMany("Columns")
                        .HasForeignKey("DataSourceId");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.DataSources.DataSource", b =>
                {
                    b.HasOne("Mlsat.Models.Entities.DataSources.DataSource", "BaseDataSource")
                        .WithMany()
                        .HasForeignKey("BaseDataSourceId");

                    b.HasOne("Mlsat.Models.Entities.Projects.Project", "Project")
                        .WithMany("DataSources")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BaseDataSource");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Models.BaseModels.Model", b =>
                {
                    b.HasOne("Mlsat.Models.Entities.DataSources.DataSource", "DataSource")
                        .WithMany()
                        .HasForeignKey("DataSourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mlsat.Models.Entities.Projects.Project", "Project")
                        .WithMany("Models")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataSource");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Models.ModelColumn", b =>
                {
                    b.HasOne("Mlsat.Models.Entities.Models.BaseModels.Model", null)
                        .WithMany("ModelColumns")
                        .HasForeignKey("ModelId");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Models.SpaceWeatherColumn", b =>
                {
                    b.HasOne("Mlsat.Models.Entities.Models.BaseModels.Model", null)
                        .WithMany("SpaceWeatherColumns")
                        .HasForeignKey("ModelId");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.DataSources.DataSource", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Models.BaseModels.Model", b =>
                {
                    b.Navigation("ModelColumns");

                    b.Navigation("SpaceWeatherColumns");
                });

            modelBuilder.Entity("Mlsat.Models.Entities.Projects.Project", b =>
                {
                    b.Navigation("DataSources");

                    b.Navigation("Models");
                });
#pragma warning restore 612, 618
        }
    }
}
