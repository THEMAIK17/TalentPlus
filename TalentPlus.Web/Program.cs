using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using TalentPlus.Infraestructure.Identity;
using TalentPlus.Infraestructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Database Configuration (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

//  Identity Configuration
builder.Services.AddIdentity<AppUser, IdentityRole>(options => 
    {
        // Development password settings 
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// --- JWT CONFIGURATION START ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

// Safety check: ensure key exists
if (string.IsNullOrEmpty(secretKey)) throw new Exception("JWT Secret is missing in appsettings.json");

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
    {
        
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddJwtBearer(options =>
    {
        // I configure the parameters to validate incoming tokens
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
//  CONFIGURE QUESTPDF LICENSE 
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddRazorPages();

//  WEB SERVICES 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // I configure the serializer to ignore cycles to prevent crashes when fetching related data
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // I define the security scheme (JWT Bearer) for Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // I require the security scheme to be used globally in Swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Register the Services
builder.Services.AddScoped<TalentPlus.Application.Interfaces.IExcelService, TalentPlus.Infraestructure.Services.ExcelService>();
builder.Services.AddScoped<TalentPlus.Application.Interfaces.IEmployeeService, TalentPlus.Infraestructure.Services.EmployeeService>();
builder.Services.AddScoped<TalentPlus.Application.Interfaces.IPdfService, TalentPlus.Infraestructure.Services.PdfService>();
builder.Services.AddScoped<TalentPlus.Application.Interfaces.IEmailService, TalentPlus.Infraestructure.Services.SmtpEmailService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();
app.MapRazorPages();

// This block executes every time the app starts to ensure DB exists and has data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        
        context.Database.Migrate();
        
        await TalentPlus.Infraestructure.Persistence.DbInitializer.SeedDataAsync(context, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB or Seeding data.");
    }
}

Console.WriteLine("\n\n");
Console.WriteLine("===========================================================");
Console.WriteLine("    TALENTPLUS SYSTEM STARTED SUCCESSFULLY!  ");
Console.WriteLine("===========================================================");
Console.WriteLine("   >  Web Login:    http://localhost:5200/Login");
Console.WriteLine("   >  Swagger API:  http://localhost:5200/swagger");
Console.WriteLine("===========================================================");
Console.WriteLine("\n\n");
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
public partial class Program { }

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
