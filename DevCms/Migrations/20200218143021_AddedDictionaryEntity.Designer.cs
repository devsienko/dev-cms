﻿// <auto-generated />
using System;
using DevCms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace devcms.Migrations
{
    [DbContext(typeof(DevCmsDb))]
    [Migration("20200218143021_AddedDictionaryEntity")]
    partial class AddedDictionaryEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099");

            modelBuilder.Entity("DevCms.ContentTypes.Attribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttrType");

                    b.Property<int>("ContentTypeId");

                    b.Property<int?>("EntityTypeId");

                    b.Property<string>("Name");

                    b.Property<bool>("Required");

                    b.HasKey("Id");

                    b.HasIndex("EntityTypeId");

                    b.ToTable("ContentAttrs");
                });

            modelBuilder.Entity("DevCms.ContentTypes.AttrValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttrId");

                    b.Property<int>("EntityId");

                    b.Property<string>("Value");

                    b.Property<int?>("ValueAsFileId");

                    b.HasKey("Id");

                    b.HasIndex("AttrId");

                    b.HasIndex("EntityId");

                    b.HasIndex("ValueAsFileId");

                    b.ToTable("AttrValues");
                });

            modelBuilder.Entity("DevCms.ContentTypes.Dictionary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Dictionaries");
                });

            modelBuilder.Entity("DevCms.ContentTypes.DictionaryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DictionaryId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.ToTable("DictionaryItems");
                });

            modelBuilder.Entity("DevCms.ContentTypes.Entity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EntityTypeId");

                    b.HasKey("Id");

                    b.HasIndex("EntityTypeId");

                    b.ToTable("Content");
                });

            modelBuilder.Entity("DevCms.ContentTypes.EntityType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ContentTypes");
                });

            modelBuilder.Entity("DevCms.ContentTypes.FileAttrValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Bytes");

                    b.Property<string>("ContentType");

                    b.Property<string>("FileName");

                    b.HasKey("Id");

                    b.ToTable("FileAttrValue");
                });

            modelBuilder.Entity("DevCms.Db.ApplicationSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NotificationRedirectionEmail")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("ApplicationSettings");
                });

            modelBuilder.Entity("DevCms.Db.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<string>("EmailStatus")
                        .HasMaxLength(255);

                    b.Property<int>("FailedLoginAttemptCounter");

                    b.Property<DateTime?>("FailedLoginAttemptDateTime");

                    b.Property<string>("Password")
                        .HasMaxLength(255);

                    b.Property<string>("PasswordSalt")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("RegistrationDateTime");

                    b.Property<string>("RegistrationStatus")
                        .HasMaxLength(255);

                    b.Property<string>("SecurityAnswer")
                        .HasMaxLength(255);

                    b.Property<string>("SecurityQuestion")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DevCms.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Email");

                    b.Property<string>("Message");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("DevCms.ContentTypes.Attribute", b =>
                {
                    b.HasOne("DevCms.ContentTypes.EntityType", "EntityType")
                        .WithMany("Attrs")
                        .HasForeignKey("EntityTypeId");
                });

            modelBuilder.Entity("DevCms.ContentTypes.AttrValue", b =>
                {
                    b.HasOne("DevCms.ContentTypes.Attribute", "Attr")
                        .WithMany()
                        .HasForeignKey("AttrId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DevCms.ContentTypes.Entity", "Entity")
                        .WithMany("AttrValues")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DevCms.ContentTypes.FileAttrValue", "ValueAsFile")
                        .WithMany()
                        .HasForeignKey("ValueAsFileId");
                });

            modelBuilder.Entity("DevCms.ContentTypes.DictionaryItem", b =>
                {
                    b.HasOne("DevCms.ContentTypes.Dictionary", "Dictionary")
                        .WithMany("Items")
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DevCms.ContentTypes.Entity", b =>
                {
                    b.HasOne("DevCms.ContentTypes.EntityType", "EntityType")
                        .WithMany()
                        .HasForeignKey("EntityTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
