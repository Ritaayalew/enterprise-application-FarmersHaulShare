using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using PricingAndFairCostSplit.Application.Services;
using PricingAndFairCostSplit.Application.Interfaces;
using PricingAndFairCostSplit.Infrastructure;
using PricingAndFairCostSplit.Infrastructure.Repositories;
using PricingAndFairCostSplit.Application.Commands;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Shared Kernel
// --------------------
builder.Services.AddScoped<IClaimsTransformation, SharedKernel.ClaimsTransformer>();

// --------------------
// Authentication (Keycloak / JWT)
// --------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/farmershaulshare";
        options.Audience = "farmershaulshare-api";
        options.RequireHttpsMetadata = false;

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

// --------------------
// Authorization Policies
// --------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Farmer", policy => policy.RequireRole("farmer"));
    options.AddPolicy("Driver", policy => policy.RequireRole("driver"));
    options.AddPolicy("Coordinator", policy => policy.RequireRole("coordinator"));
    options.AddPolicy("Buyer", policy => policy.RequireRole("buyer"));
});

// --------------------
// Controllers & Swagger
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// EF Core (In-Memory for now)
// --------------------
builder.Services.AddDbContext<PricingDbContext>(options =>
{
    options.UseInMemoryDatabase("PricingDb");
});

// --------------------
// Dependency Injection
// --------------------
builder.Services.AddScoped<IFairCostSplitRepository, FairCostSplitRepository>();
builder.Services.AddScoped<IFairPricingAppService, FairPricingAppService>();

var app = builder.Build();

// --------------------
// Swagger in Development
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pricing & Fair Cost Split API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

// --------------------
// Middleware pipeline
// --------------------
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// --------------------
// Test endpoints
// --------------------
app.MapGet("/public", () => "Anyone can access this!");
app.MapGet("/protected", [Authorize] () => "Welcome! You are authenticated.");
app.MapGet("/farmer-only", [Authorize(Policy = "Farmer")] () => "Hello Farmer! 🌾");
app.MapGet("/driver-only", [Authorize(Policy = "Driver")] () => "Hello Driver! 🚛");
app.MapGet("/coordinator-only", [Authorize(Policy = "Coordinator")] () => "Hello Coordinator! 👨‍💼");

// --------------------
// Minimal API for fair cost calculation
// --------------------
app.MapPost("/calculate-fair-cost", async (CalculateFairCostSplitCommand command, IFairPricingAppService service) =>
{
    var result = await service.CalculateFairCostSplit(command);
    return Results.Ok(result);
});

app.MapControllers();

app.Run();
