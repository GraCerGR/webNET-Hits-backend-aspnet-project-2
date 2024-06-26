﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Test.Models;

#nullable disable

namespace Test.Migrations
{
    [DbContext(typeof(TestContext))]
    [Migration("20231209095342_AddFixType")]
    partial class AddFixType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Test.Models.DTO.PostDto", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("addressId")
                        .HasColumnType("uuid");

                    b.Property<string>("author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("authorId")
                        .HasColumnType("uuid");

                    b.Property<int>("commentsCount")
                        .HasColumnType("integer");

                    b.Property<Guid?>("communityId")
                        .HasColumnType("uuid");

                    b.Property<string>("communityName")
                        .HasColumnType("text");

                    b.Property<string>("createTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("hasLike")
                        .HasColumnType("boolean");

                    b.Property<string>("image")
                        .HasColumnType("text");

                    b.Property<int>("likes")
                        .HasColumnType("integer");

                    b.Property<int>("readingTime")
                        .HasColumnType("integer");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Test.Models.DTO.PostTag", b =>
                {
                    b.Property<Guid>("postId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("tagId")
                        .HasColumnType("uuid");

                    b.HasKey("postId", "tagId");

                    b.ToTable("PostTags");
                });

            modelBuilder.Entity("Test.Models.DTO.TagDto", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PostDtoid")
                        .HasColumnType("uuid");

                    b.Property<string>("createTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.HasIndex("PostDtoid");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Test.Models.User", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("birthDate")
                        .HasColumnType("text");

                    b.Property<string>("createTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("fullName")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<int>("gender")
                        .HasColumnType("integer");

                    b.Property<int>("likes")
                        .HasColumnType("integer");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("posts")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Test.Models.DTO.TagDto", b =>
                {
                    b.HasOne("Test.Models.DTO.PostDto", null)
                        .WithMany("tags")
                        .HasForeignKey("PostDtoid");
                });

            modelBuilder.Entity("Test.Models.DTO.PostDto", b =>
                {
                    b.Navigation("tags");
                });
#pragma warning restore 612, 618
        }
    }
}
