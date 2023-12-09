﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Models.DTO;

namespace Test.Models
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options): base(options)
        {
            //Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }

        public DbSet<PostDto> Posts { get; set; }

        public DbSet<TagDto> Tags { get; set; }

        public DbSet<PostTag> PostTags { get; set; }

        public DbSet<PostLiked> PostLikes { get; set; }

        public DbSet<CommunityFullDto> Communities { get; set; }

        public DbSet<CommunityUser> CommunityUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.id);

            modelBuilder.Entity<PostDto>().HasKey(x => x.id);

            modelBuilder.Entity<TagDto>().HasKey(x => x.id);

            modelBuilder.Entity<PostTag>().HasKey(x => new { x.postId, x.tagId });

            modelBuilder.Entity<PostLiked>().HasKey(x => new { x.userId, x.postId });

            modelBuilder.Entity<CommunityFullDto>().HasKey(x => x.id);

            modelBuilder.Entity<CommunityUser>().HasKey(x => new { x.communityId, x.userId });


            modelBuilder.Entity<User>(options => { });

            modelBuilder.Entity<PostDto>(options => { });

            modelBuilder.Entity<TagDto>(options => { });

            modelBuilder.Entity<PostTag>(options => { });

            modelBuilder.Entity<PostLiked>(options => { });

            modelBuilder.Entity<CommunityFullDto>(options => { });

            modelBuilder.Entity<CommunityUser>(options => { });

            base.OnModelCreating(modelBuilder);

        }
    }

}
