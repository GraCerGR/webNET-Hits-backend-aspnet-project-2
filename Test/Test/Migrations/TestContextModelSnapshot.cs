﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Test.Models;

#nullable disable

namespace Test.Migrations
{
    [DbContext(typeof(TestContext))]
    partial class TestContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Test.Models.DTO.PostDto", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("text");

                    b.Property<string>("addressId")
                        .HasColumnType("text");

                    b.Property<string>("author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("authorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("commentsCount")
                        .HasColumnType("integer");

                    b.Property<string>("communityId")
                        .HasColumnType("text");

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

                    b.Property<string>("tagsid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.HasIndex("tagsid");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Test.Models.DTO.PostTag", b =>
                {
                    b.Property<string>("postId")
                        .HasColumnType("text");

                    b.Property<string>("tagId")
                        .HasColumnType("text");

                    b.HasKey("postId", "tagId");

                    b.ToTable("PostTags");
                });

            modelBuilder.Entity("Test.Models.DTO.TagDto", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("text");

                    b.Property<string>("createTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Test.Models.User", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("text");

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

                    b.Property<string>("gender")
                        .IsRequired()
                        .HasColumnType("text");

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

            modelBuilder.Entity("Test.Models.DTO.PostDto", b =>
                {
                    b.HasOne("Test.Models.DTO.TagDto", "tags")
                        .WithMany()
                        .HasForeignKey("tagsid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("tags");
                });
#pragma warning restore 612, 618
        }
    }
}
