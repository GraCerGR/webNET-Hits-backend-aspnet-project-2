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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.id);

            modelBuilder.Entity<PostDto>().HasKey(x => x.id);

            modelBuilder.Entity<TagDto>().HasKey(x => x.id);

            modelBuilder.Entity<User>(options => { });

            modelBuilder.Entity<PostDto>(options => { });

            modelBuilder.Entity<TagDto>(options => { });

            base.OnModelCreating(modelBuilder);

        }
    }

}
