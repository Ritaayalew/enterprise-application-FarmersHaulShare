using BatchPostingAndGrouping.Application.Services;
using BatchPostingAndGrouping.Domain.Repositories;
using BatchPostingAndGrouping.Infrastructure.Data;
using BatchPostingAndGrouping.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClaimsTransformation, SharedKernel.ClaimsTransformer>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/farmershaulshare";
        options.Audience = "farmershaulshare-api";  // client ID
        options.RequireHttpsMetadata = false;  // Required for http in dev
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:8080/realms/farmershaulshare",
            ValidAudience = "farmershaulshare-api"
        };
        options.MapInboundClaims = false;
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Farmer", policy => policy.RequireRole("farmer"));
    options.AddPolicy("Driver", policy => policy.RequireRole("driver"));
    options.AddPolicy("Coordinator", policy => policy.RequireRole("coordinator"));
    options.AddPolicy("Buyer", policy => policy.RequireRole("buyer"));
});

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("BatchPostingDb")
    ?? "Server=localhost;Database=FarmersHaulShare_BatchPosting;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<BatchPostingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories
builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<IFarmerProfileRepository, FarmerProfileRepository>();
builder.Services.AddScoped<IGroupCandidateRepository, GroupCandidateRepository>();

// Register application services
builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddScoped<IGroupingService, GroupingService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Ensure database is created (in development only)
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BatchPostingDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// EASY TEST ENDPOINTS
app.MapGet("/public", () => "Anyone can access this!");

// Requires login (any valid token)
app.MapGet("/protected", [Authorize] () => $"Welcome! You are authenticated.");

// Requires specific role
app.MapGet("/farmer-only", [Authorize(Policy = "Farmer")] () => "Hello Farmer! 🌾");
app.MapGet("/driver-only", [Authorize(Policy = "Driver")] () => "Hello Driver! 🚛");
app.MapGet("/coordinator-only", [Authorize(Policy = "Coordinator")] () => "Hello Coordinator! 👨‍💼");

app.Run();