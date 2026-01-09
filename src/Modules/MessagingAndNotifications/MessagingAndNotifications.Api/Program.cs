using MessagingAndNotifications.Application.EventHandlers;
using MessagingAndNotifications.Application.Services;
using MessagingAndNotifications.Domain.Repositories;
using MessagingAndNotifications.Infrastructure.Consumers;
using MessagingAndNotifications.Infrastructure.Data;
using MessagingAndNotifications.Infrastructure.Repositories;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Infrastructure.Repositories;
using TransportMarketplaceAndDispatch.Infrastructure.Persistence;
using HaulShareCreationAndScheduling.Infrastructure.Persistence;
using BatchPostingAndGrouping.Domain.Repositories;
using BatchPostingAndGrouping.Infrastructure.Repositories;
using BatchPostingAndGrouping.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IClaimsTransformation, SharedKernel.ClaimsTransformer>();


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

// Database - Messaging Module
var messagingConnectionString = builder.Configuration.GetConnectionString("MessagingDb")
    ?? "Server=localhost;Database=FarmersHaulShare_Messaging;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<MessagingDbContext>(options =>
    options.UseSqlServer(messagingConnectionString));


builder.Services.AddDbContext<HaulShareDbContext>(options =>
    options.UseInMemoryDatabase("HaulShareDb"));

builder.Services.AddDbContext<TransportDbContext>(options =>
    options.UseInMemoryDatabase("TransportDb"));


var batchPostingConnectionString = builder.Configuration.GetConnectionString("BatchPostingDb")
    ?? "Server=localhost;Database=FarmersHaulShare_BatchPosting;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<BatchPostingDbContext>(options =>
    options.UseSqlServer(batchPostingConnectionString));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


builder.Services.AddScoped<IDispatchJobRepository, DispatchJobRepository>();
builder.Services.AddScoped<IFarmerProfileRepository, FarmerProfileRepository>();


builder.Services.AddScoped<ITemplateService, InMemoryTemplateService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddScoped<IQuoteEventHandler, QuoteEventHandler>();
builder.Services.AddScoped<IStatusUpdateEventHandler, StatusUpdateEventHandler>();
builder.Services.AddScoped<IReceiptEventHandler, ReceiptEventHandler>();


var massTransitConfig = builder.Configuration.GetSection("MassTransit:RabbitMq");
builder.Services.AddMassTransit(x =>
{

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

      
        cfg.ConfigureEndpoints(context);

     
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapGet("/public", () => "Anyone can access this!");
app.MapGet("/protected", [Authorize] () => $"Welcome! You are authenticated.");
app.MapGet("/farmer-only", [Authorize(Policy = "Farmer")] () => "Hello Farmer! 🌾");
app.MapGet("/driver-only", [Authorize(Policy = "Driver")] () => "Hello Driver! 🚛");
app.MapGet("/coordinator-only", [Authorize(Policy = "Coordinator")] () => "Hello Coordinator! 👨‍💼");

app.Run();
