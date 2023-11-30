using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Models
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options): base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<UserDto> Users { get; set; }

        public DbSet<UserRegisterModel> UserRegisterModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDto>().HasKey(x => x.id);

            modelBuilder.Entity<UserRegisterModel>().HasKey(x => x.fullname);

            modelBuilder.Entity<UserRegisterModel>(options =>
            {

            });

            modelBuilder.Entity<UserDto>(options =>
            {

            });
            base.OnModelCreating(modelBuilder);

        }
    }
}
