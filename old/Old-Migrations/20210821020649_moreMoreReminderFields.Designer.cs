﻿// <auto-generated />
using System;
using ClassLibrary.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClassLibrary.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210821020649_moreMoreReminderFields")]
    partial class moreMoreReminderFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ClassLibrary.Models.CryptoModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("coinName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("dateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<float>("price1")
                        .HasColumnType("real");

                    b.Property<float>("price2")
                        .HasColumnType("real");

                    b.Property<float>("price3")
                        .HasColumnType("real");

                    b.Property<float>("price4")
                        .HasColumnType("real");

                    b.Property<float>("price5")
                        .HasColumnType("real");

                    b.HasKey("id");

                    b.ToTable("CryptoModels");
                });

            modelBuilder.Entity("ClassLibrary.Models.DashItem", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("UserDashid")
                        .HasColumnType("int");

                    b.Property<int>("command")
                        .HasColumnType("int");

                    b.Property<string>("result")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("serverId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("userId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("UserDashid");

                    b.ToTable("DashItems");
                });

            modelBuilder.Entity("ClassLibrary.Models.ReminderModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("additionalInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("createdById")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("createdInServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int?>("currentRepeatNumber")
                        .HasColumnType("int");

                    b.Property<bool?>("endDate")
                        .HasColumnType("bit");

                    b.Property<DateTime>("executionTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("hasExecuted")
                        .HasColumnType("bit");

                    b.Property<int?>("numberOfRepeats")
                        .HasColumnType("int");

                    b.Property<Guid>("reminderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("repeatIncrement")
                        .HasColumnType("int");

                    b.Property<bool?>("shouldRepeat")
                        .HasColumnType("bit");

                    b.Property<DateTime>("timeAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("ReminderModels");
                });

            modelBuilder.Entity("ClassLibrary.Models.ServerModel", b =>
                {
                    b.Property<decimal>("serverId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("serverName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("serverId");

                    b.ToTable("ServerModels");
                });

            modelBuilder.Entity("ClassLibrary.Models.StatModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("bank")
                        .HasColumnType("int");

                    b.Property<int>("emojiSent")
                        .HasColumnType("int");

                    b.Property<float>("experience")
                        .HasColumnType("real");

                    b.Property<int>("luck")
                        .HasColumnType("int");

                    b.Property<int>("messages")
                        .HasColumnType("int");

                    b.Property<int>("reactionsReceived")
                        .HasColumnType("int");

                    b.Property<decimal>("serverId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("userId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("userLevel")
                        .HasColumnType("int");

                    b.Property<int>("wallet")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("StatModels");
                });

            modelBuilder.Entity("ClassLibrary.Models.UserDash", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("color")
                        .HasColumnType("int");

                    b.Property<decimal>("serverId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("userId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("userName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("UserDashes");
                });

            modelBuilder.Entity("ClassLibrary.Models.UserExperience", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("bank")
                        .HasColumnType("int");

                    b.Property<DateTime>("dateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("emojiSent")
                        .HasColumnType("int");

                    b.Property<float>("experience")
                        .HasColumnType("real");

                    b.Property<int>("luck")
                        .HasColumnType("int");

                    b.Property<int>("messages")
                        .HasColumnType("int");

                    b.Property<int>("reactionsReceived")
                        .HasColumnType("int");

                    b.Property<decimal>("serverId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("userId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("userLevel")
                        .HasColumnType("int");

                    b.Property<string>("userName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("wallet")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("UserExperiences");
                });

            modelBuilder.Entity("ClassLibrary.Models.UserModel", b =>
                {
                    b.Property<decimal>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("slowModeEnabled")
                        .HasColumnType("bit");

                    b.Property<int>("slowModeTime")
                        .HasColumnType("int");

                    b.Property<string>("userName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("userId");

                    b.ToTable("UserModels");
                });

            modelBuilder.Entity("ClassLibrary.Models.UserStatsModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("serverId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int?>("statsid")
                        .HasColumnType("int");

                    b.Property<decimal>("userId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("id");

                    b.HasIndex("statsid");

                    b.ToTable("UserStatModels");
                });

            modelBuilder.Entity("ClassLibrary.Models.DashItem", b =>
                {
                    b.HasOne("ClassLibrary.Models.UserDash", null)
                        .WithMany("items")
                        .HasForeignKey("UserDashid");
                });

            modelBuilder.Entity("ClassLibrary.Models.UserStatsModel", b =>
                {
                    b.HasOne("ClassLibrary.Models.StatModel", "stats")
                        .WithMany()
                        .HasForeignKey("statsid");

                    b.Navigation("stats");
                });

            modelBuilder.Entity("ClassLibrary.Models.UserDash", b =>
                {
                    b.Navigation("items");
                });
#pragma warning restore 612, 618
        }
    }
}
