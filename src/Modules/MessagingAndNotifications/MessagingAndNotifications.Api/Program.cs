using MessagingAndNotifications.Application.EventHandlers;
using MessagingAndNotifications.Application.Services;
using MessagingAndNotifications.Domain.Repositories;
using MessagingAndNotifications.Infrastructure.Consumers;
using MessagingAndNotifications.Infrastructure.Data;
using MessagingAndNotifications.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IClaimsTransformation, SharedKernel.ClaimsTransformer>();

// JWT Authentication
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

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Farmer", policy => policy.RequireRole("farmer"));
    options.AddPolicy("Driver", policy => policy.RequireRole("driver"));
    options.AddPolicy("Coordinator", policy => policy.RequireRole("coordinator"));
    options.AddPolicy("Buyer", policy => policy.RequireRole("buyer"));
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("MessagingDb")
    ?? "Server=localhost;Database=FarmersHaulShare_Messaging;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<MessagingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Application Services
builder.Services.AddScoped<ITemplateService, InMemoryTemplateService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Event Handlers
builder.Services.AddScoped<IQuoteEventHandler, QuoteEventHandler>();
builder.Services.AddScoped<IStatusUpdateEventHandler, StatusUpdateEventHandler>();
builder.Services.AddScoped<IReceiptEventHandler, ReceiptEventHandler>();

// MassTransit (RabbitMQ)
var massTransitConfig = builder.Configuration.GetSection("MassTransit:RabbitMq");
builder.Services.AddMassTransit(x =>
{
    // Add consumers
    x.AddConsumer<QuoteEventConsumer>();
    x.AddConsumer<PickupStartedConsumer>();
    x.AddConsumer<PickupCompletedConsumer>();
    x.AddConsumer<DeliveryStartedConsumer>();
    x.AddConsumer<DeliveryCompletedConsumer>();
    x.AddConsumer<ReceiptEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            massTransitConfig["Host"] ?? "localhost",
            h =>
            {
                h.Username(massTransitConfig["Username"] ?? "guest");
                h.Password(massTransitConfig["Password"] ?? "guest");
            });

        // Configure consumers
        cfg.ConfigureEndpoints(context);

        // Note: When other modules are implemented, configure specific queues:
        // cfg.ReceiveEndpoint("quote-events", e => { e.ConfigureConsumer<QuoteEventConsumer>(context); });
        // cfg.ReceiveEndpoint("status-events", e => { e.ConfigureConsumer<PickupStartedConsumer>(context); });
        // etc.
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Ensure database is created (in development only)
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Test endpoints
app.MapGet("/public", () => "Anyone can access this!");
app.MapGet("/protected", [Authorize] () => $"Welcome! You are authenticated.");
app.MapGet("/farmer-only", [Authorize(Policy = "Farmer")] () => "Hello Farmer! 🌾");
app.MapGet("/driver-only", [Authorize(Policy = "Driver")] () => "Hello Driver! 🚛");
app.MapGet("/coordinator-only", [Authorize(Policy = "Coordinator")] () => "Hello Coordinator! 👨‍💼");

app.Run();
