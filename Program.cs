using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using todo_web_api.Controllers;
using todo_web_api.Data;
using todo_web_api.Helpers;
using todo_web_api.Interface;
using todo_web_api.Models;
using todo_web_api.Respository;
using todo_web_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// var config = builder.Configuration;

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddControllers();

// cors
// builder.Services.AddCors();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", builder =>
    {
        builder.WithOrigins(
            "http://localhost:4200", // Angular dev
            "https://localhost:4200" // in case Angular is served via HTTPS later
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // only if you use cookies or sessions
    });
});

// Register ApplicationDbContext for SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connStr) // UseSqlServer for SQL Server
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information)
);

// identity 
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidIssuer = "http://localhost:5288",
        ValidateAudience = false,
        ValidAudience = "http://localhost:5288",
        ValidateIssuerSigningKey = false,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("sdgfijjh3466iu345g87g08c24g7204gr803g30587ghh35807fg39074fvg80493745gf082b507807g807fgf")
        )
    };
});


// Dependency Injections
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<UserServiceTrail>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ITodoRespository, TodoRespository>();

// configured the cloudinary settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


// below is a pipline middleware
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}


// to serve the angualar static files(coupled version)
var angularDistPath = Path.Combine(Directory.GetCurrentDirectory(), "client", "test", "dist", "test");
Console.WriteLine($"Serving Angular from: {angularDistPath}");

// Serve default files (like index.html)
app.UseDefaultFiles();

// Serve static files (js, css, images) , to server angular production data
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(angularDistPath),
//     RequestPath = ""
// });


// Enable static files serving , to serve the images which is stored inside assets/images
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("AllowAngularClient");

app.UseAuthentication(); // this MUST be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

// Basic route for /
app.MapGet("/api/status", () => "API is live");

// This should come after all other route mappings
// This is important to serve index.html for react routing
app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(angularDistPath)
});

app.Run();
