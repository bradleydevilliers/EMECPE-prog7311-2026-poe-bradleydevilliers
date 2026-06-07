using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechMoveGLMS.Shared.Data;
using TechMoveGLMS.Shared.Services;
using TechMoveGLMS.Shared.Services.Contracts;
using TechMoveGLMS.Shared.Services.Notifications;
using TechMoveGLMS.Shared.Services.Pricing;
using TechMoveGLMS.API.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services
builder.Services.AddControllers()
 .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddHttpContextAccessor();   // ← REQUIRED for AuthService
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HTTP client for CurrencyService
builder.Services.AddHttpClient<CurrencyService>();

// Design patterns
builder.Services.AddScoped<IContractFactory, ContractFactory>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddScoped<INotificationObserver, EmailNotifier>();
builder.Services.AddScoped<INotificationObserver, ComplianceLogger>();
builder.Services.AddScoped<PricingContext>();

// AuthService (for user creation and admin check)
builder.Services.AddScoped<AuthService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure database and default admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    var auth = scope.ServiceProvider.GetRequiredService<AuthService>();
    await auth.EnsureAdminExistsAsync();
}

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