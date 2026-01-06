using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using HaulShareCreationAndScheduling.Application.Commands;
using HaulShareCreationAndScheduling.Application.Handlers;
using HaulShareCreationAndScheduling.Domain.Repositories;
using HaulShareCreationAndScheduling.Infrastructure.Persistence;
using HaulShareCreationAndScheduling.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

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


// =========================
// HaulShare module services
// =========================
builder.Services.AddDbContext<HaulShareDbContext>(options =>
    options.UseInMemoryDatabase("HaulShareDb"));

builder.Services.AddScoped<IHaulShareRepository, HaulShareRepository>();
builder.Services.AddScoped<CreateHaulShareFromLockedGroupHandler>();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// HaulShare endpoints
// =========================
app.MapPost(
    "/haulshares",
    [Authorize(Policy = "Coordinator")]
    async (
        CreateHaulShareFromLockedGroupCommand command,
        CreateHaulShareFromLockedGroupHandler handler,
        CancellationToken ct) =>
    {
        var id = await handler.Handle(command, ct);
        return Results.Created($"/haulshares/{id}", id);
    });


// EASY TEST ENDPOINTS
app.MapGet("/public", () => "Anyone can access this!");

// Requires login (any valid token)
app.MapGet("/protected", [Authorize] () => $"Welcome! You are authenticated.");

// Requires specific role
app.MapGet("/farmer-only", [Authorize(Policy = "Farmer")] () => "Hello Farmer! 🌾");
app.MapGet("/driver-only", [Authorize(Policy = "Driver")] () => "Hello Driver! 🚛");
app.MapGet("/coordinator-only", [Authorize(Policy = "Coordinator")] () => "Hello Coordinator! 👨‍💼");

app.Run();