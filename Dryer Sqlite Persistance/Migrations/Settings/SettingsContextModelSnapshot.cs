﻿// <auto-generated />
using System;
using Dryer_Server.Persistance.Model.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dryer_Sqlite_Persistance.Migrations.Settings
{
    [DbContext(typeof(SettingsContext))]
    partial class SettingsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Dryer_Server.Persistance.Model.Settings.ChamberSetting", b =>
                {
                    b.Property<DateTime>("CreationTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InFlowActuatorNo")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OutFlowActuatorNo")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SensorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThroughFlowActuatorNo")
                        .HasColumnType("INTEGER");

                    b.HasIndex("Id", "CreationTimeUtc")
                        .IsUnique();

                    b.ToTable("Chamber");
                });
#pragma warning restore 612, 618
        }
    }
}
