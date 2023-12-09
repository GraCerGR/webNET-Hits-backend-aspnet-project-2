using Microsoft.AspNetCore.Authorization;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.id);

            modelBuilder.Entity<PostDto>().HasKey(x => x.id);

            modelBuilder.Entity<TagDto>().HasKey(x => x.id);

            modelBuilder.Entity<PostTag>().HasKey(x => new { x.postId, x.tagId });


            modelBuilder.Entity<User>(options => { });

            modelBuilder.Entity<PostDto>(options => { });

            modelBuilder.Entity<TagDto>(options => { });

            modelBuilder.Entity<PostTag>(options => { });

            base.OnModelCreating(modelBuilder);

        }
    }

}
