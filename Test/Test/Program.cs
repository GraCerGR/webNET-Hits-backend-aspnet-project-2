using Test.Models;
using Microsoft.EntityFrameworkCore;
using Test.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//DB:
builder.Services.AddDbContext<TestContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация ITokenService
//builder.Services.AddScoped<ITokenService, TokenService>();
//builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//Токен
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "HITS",
        ValidateIssuer = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456789012345678901234567890")),
        ValidateLifetime = true,
        LifetimeValidator = (before, expires, token, parameters) =>
        {
            var utcNow = DateTime.UtcNow;
            return before <= utcNow && utcNow <= expires;
        }
    };
});


var app = builder.Build();

//DB init and uodate:
using var serviseScope = app.Services.CreateScope();
var dbContext = serviseScope.ServiceProvider.GetService<TestContext>();
dbContext?.Database.Migrate(); //Migration

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
