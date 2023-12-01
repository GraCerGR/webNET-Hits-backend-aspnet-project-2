using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Models.DTO;

namespace Test.Models
{
    public class TestContext : DbContext
    {

        [HttpGet]
        [AllowAnonymous] // Разрешить доступ без авторизации
        public ActionResult<string> GetPublic()
        {
            return "Это открытый эндпоинт.";
        }

        [HttpGet]
        [Authorize] // Требовать авторизацию для этого эндпоинта
        public ActionResult<string> GetAuthorized()
        {
            return "Это авторизованный эндпоинт.";
        }
        public TestContext(DbContextOptions<TestContext> options): base(options)
        {
            //Database.EnsureCreated();
        }

        //public DbSet<UserDto> Users { get; set; }

        //public DbSet<UserRegisterModel> UserRegisterModels { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.id);

/*            modelBuilder.Entity<UserRegisterModel>().HasKey(x => x.fullname);

            modelBuilder.Entity<UserRegisterModel>(options =>
            {

            });*/

            modelBuilder.Entity<User>(options =>
            {

            });
            base.OnModelCreating(modelBuilder);

        }
    }
}
