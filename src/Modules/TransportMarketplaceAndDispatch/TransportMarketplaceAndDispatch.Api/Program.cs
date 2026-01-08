using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MassTransit;
using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Application.Handlers;
using TransportMarketplaceAndDispatch.Application.Queries;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Infrastructure.Persistence;
using TransportMarketplaceAndDispatch.Infrastructure.Repositories;
using TransportMarketplaceAndDispatch.Infrastructure.Consumers;

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

// Database
builder.Services.AddDbContext<TransportDbContext>(options =>
    options.UseInMemoryDatabase("TransportDb"));

// Repositories
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IDispatchJobRepository, DispatchJobRepository>();

// Application Handlers
builder.Services.AddScoped<RegisterDriverHandler>();
builder.Services.AddScoped<AddVehicleHandler>();
builder.Services.AddScoped<VerifyVehicleHandler>();
builder.Services.AddScoped<UpdateAvailabilityHandler>();
builder.Services.AddScoped<PostDispatchJobHandler>();
builder.Services.AddScoped<AcceptDispatchJobHandler>();
builder.Services.AddScoped<RecordGeofencePingHandler>();
builder.Services.AddScoped<StartPickupHandler>();
builder.Services.AddScoped<CompletePickupHandler>();
builder.Services.AddScoped<StartDeliveryHandler>();
builder.Services.AddScoped<CompleteDeliveryHandler>();
builder.Services.AddScoped<GetAvailableDriversQueryHandler>();
builder.Services.AddScoped<GetDriverJobsQueryHandler>();
builder.Services.AddScoped<GetJobStatusQueryHandler>();
builder.Services.AddScoped<TransportMarketplaceAndDispatch.Application.Handlers.GetAvailableJobsQueryHandler>();

// MassTransit (RabbitMQ) for event consumption
var massTransitConfig = builder.Configuration.GetSection("MassTransit:RabbitMq");
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HaulShareCreatedConsumer>();

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

app.UseAuthentication();
app.UseAuthorization();

// =========================
// Driver endpoints
// =========================

// Register driver (public or coordinator only)
app.MapPost("/drivers",
    [Authorize(Policy = "Coordinator")]
    async (
        RegisterDriverCommand command,
        RegisterDriverHandler handler,
        CancellationToken ct) =>
    {
        var id = await handler.Handle(command, ct);
        return Results.Created($"/drivers/{id}", new { Id = id });
    });

// Update driver availability (driver only)
app.MapPut("/drivers/{driverId}/availability",
    [Authorize(Policy = "Driver")]
    async (
        Guid driverId,
        UpdateAvailabilityCommand command,
        UpdateAvailabilityHandler handler,
        CancellationToken ct) =>
    {
        if (driverId != command.DriverId)
            return Results.BadRequest("Driver ID mismatch");

        await handler.Handle(command, ct);
        return Results.Ok();
    });

// Add vehicle to driver (driver only)
app.MapPost("/drivers/{driverId}/vehicles",
    [Authorize(Policy = "Driver")]
    async (
        Guid driverId,
        AddVehicleCommand command,
        AddVehicleHandler handler,
        CancellationToken ct) =>
    {
        if (driverId != command.DriverId)
            return Results.BadRequest("Driver ID mismatch");

        var vehicleId = await handler.Handle(command, ct);
        return Results.Created($"/drivers/{driverId}/vehicles/{vehicleId}", new { Id = vehicleId });
    });

// Verify vehicle (coordinator only)
app.MapPost("/drivers/{driverId}/vehicles/{vehicleId}/verify",
    [Authorize(Policy = "Coordinator")]
    async (
        Guid driverId,
        Guid vehicleId,
        VerifyVehicleHandler handler,
        CancellationToken ct) =>
    {
        await handler.Handle(new VerifyVehicleCommand { DriverId = driverId, VehicleId = vehicleId }, ct);
        return Results.Ok();
    });

// Get available drivers (coordinator or driver)
app.MapGet("/drivers/available",
    [Authorize(Policy = "Coordinator")]
    async (
        [AsParameters] GetAvailableDriversQuery query,
        GetAvailableDriversQueryHandler handler,
        CancellationToken ct) =>
    {
        var result = await handler.Handle(query, ct);
        return Results.Ok(result);
    });

// =========================
// Dispatch Job endpoints
// =========================

// Post dispatch job (system/internal - typically called by event handler)
app.MapPost("/dispatch-jobs",
    [Authorize(Policy = "Coordinator")]
    async (
        PostDispatchJobCommand command,
        PostDispatchJobHandler handler,
        CancellationToken ct) =>
    {
        var id = await handler.Handle(command, ct);
        return Results.Created($"/dispatch-jobs/{id}", new { Id = id });
    });

// Get available jobs (driver only)
app.MapGet("/dispatch-jobs/available",
    [Authorize(Policy = "Driver")]
    async (
        TransportMarketplaceAndDispatch.Application.Handlers.GetAvailableJobsQueryHandler handler,
        CancellationToken ct) =>
    {
        var result = await handler.Handle(new GetAvailableJobsQuery(), ct);
        return Results.Ok(result);
    });

// Accept dispatch job (driver only)
app.MapPost("/dispatch-jobs/{jobId}/accept",
    [Authorize(Policy = "Driver")]
    async (
        Guid jobId,
        AcceptDispatchJobCommand command,
        AcceptDispatchJobHandler handler,
        CancellationToken ct) =>
    {
        if (jobId != command.DispatchJobId)
            return Results.BadRequest("Job ID mismatch");

        await handler.Handle(command, ct);
        return Results.Ok();
    });

// Get driver's jobs (driver only)
app.MapGet("/drivers/{driverId}/jobs",
    [Authorize(Policy = "Driver")]
    async (
        Guid driverId,
        [AsParameters] GetDriverJobsQuery query,
        GetDriverJobsQueryHandler handler,
        CancellationToken ct) =>
    {
        if (driverId != query.DriverId)
            return Results.BadRequest("Driver ID mismatch");

        var result = await handler.Handle(query, ct);
        return Results.Ok(result);
    });

// Get job status (driver, coordinator, farmer, buyer)
app.MapGet("/dispatch-jobs/{jobId}",
    [Authorize]
    async (
        Guid jobId,
        GetJobStatusQueryHandler handler,
        CancellationToken ct) =>
    {
        var result = await handler.Handle(new GetJobStatusQuery { DispatchJobId = jobId }, ct);
        if (result.Job == null)
            return Results.NotFound();

        return Results.Ok(result);
    });

// Record geofence ping (driver only)
app.MapPost("/dispatch-jobs/{jobId}/geofence-ping",
    [Authorize(Policy = "Driver")]
    async (
        Guid jobId,
        RecordGeofencePingCommand command,
        RecordGeofencePingHandler handler,
        CancellationToken ct) =>
    {
        if (jobId != command.DispatchJobId)
            return Results.BadRequest("Job ID mismatch");

        await handler.Handle(command, ct);
        return Results.Ok();
    });

// Start pickup (driver only)
app.MapPost("/dispatch-jobs/{jobId}/start-pickup",
    [Authorize(Policy = "Driver")]
    async (
        Guid jobId,
        StartPickupCommand command,
        StartPickupHandler handler,
        CancellationToken ct) =>
    {
        if (jobId != command.DispatchJobId)
            return Results.BadRequest("Job ID mismatch");

        await handler.Handle(command, ct);
        return Results.Ok();
    });

// Complete pickup (driver only)
app.MapPost("/dispatch-jobs/{jobId}/complete-pickup",
    [Authorize(Policy = "Driver")]
    async (
        Guid jobId,
        CompletePickupCommand command,
        CompletePickupHandler handler,
        CancellationToken ct) =>
    {
        if (jobId != command.DispatchJobId)
            return Results.BadRequest("Job ID mismatch");

        await handler.Handle(command, ct);
        return Results.Ok();
    });

// Start delivery (driver only)
app.MapPost("/dispatch-jobs/{jobId}/start-delivery",
    [Authorize(Policy = "Driver")]
    async (
        Guid jobId,
        StartDeliveryCommand command,
        StartDeliveryHandler handler,
        CancellationToken ct) =>
    {
        if (jobId != command.DispatchJobId)
            return Results.BadRequest("Job ID mismatch");

        await handler.Handle(command, ct);
        return Results.Ok();
    });

// Complete delivery (driver only)
app.MapPost("/dispatch-jobs/{jobId}/complete-delivery",
    [Authorize(Policy = "Driver")]
    async (
        Guid jobId,
        CompleteDeliveryCommand command,
        CompleteDeliveryHandler handler,
        CancellationToken ct) =>
    {
        if (jobId != command.DispatchJobId)
            return Results.BadRequest("Job ID mismatch");

        await handler.Handle(command, ct);
        return Results.Ok();
    });

// =========================
// Test endpoints
// =========================
app.MapGet("/public", () => "Anyone can access this!");
app.MapGet("/protected", [Authorize] () => "Welcome! You are authenticated.");
app.MapGet("/driver-only", [Authorize(Policy = "Driver")] () => "Hello Driver! 🚛");
app.MapGet("/coordinator-only", [Authorize(Policy = "Coordinator")] () => "Hello Coordinator! 👨‍💼");

app.Run();