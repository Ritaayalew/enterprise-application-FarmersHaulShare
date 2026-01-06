using FarmersHaulShare.BatchPosting.Infrastructure;
using FarmersHaulShare.Api;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BatchPostingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BatchPostingDb")));

builder.Services.AddScoped<IBatchRepository, BatchRepository>();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// Quartz background job
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("OutboxPublisherJob");
    q.AddJob<OutboxPublisherJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("OutboxPublisherTrigger")
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();