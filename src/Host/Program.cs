using Microsoft.EntityFrameworkCore;
using Quartz;
using SalesLoan.Domain.Repositories;
using SalesLoan.Infrastructure.Data;
using SalesLoan.Infrastructure.Repositories;
using Shared.Domain.EventHandlers;
using Shared.Infrastructure.EventBus;
using Shared.Infrastructure.Outbox;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Quartz.NET for Outbox Publisher Job
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjection();
    q.AddOutboxPublisherJob(intervalSeconds: 30);
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Entity Framework - Shared database with schema separation
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Server=localhost,1433;Database=RealEstateManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

// Outbox
builder.Services.AddDbContext<OutboxDbContext>(options =>
    options.UseSqlServer(connectionString));

// SalesLoan DbContext and Repositories (needed for event handlers)
builder.Services.AddDbContext<SalesLoanDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

// Event Bus
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// Outbox Publisher
builder.Services.AddScoped<OutboxPublisher>();

// RabbitMQ Event Subscriber
builder.Services.AddHostedService<RabbitMQEventSubscriber>();

// Register all event handlers
var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => a.FullName?.Contains("PropertyManagement") == true 
             || a.FullName?.Contains("PricingValuation") == true
             || a.FullName?.Contains("SalesLoan") == true);

foreach (var assembly in assemblies)
{
    var subscriberTypes = assembly.GetTypes()
        .Where(t => t.GetInterfaces().Any(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>)))
        .ToList();

    foreach (var subscriberType in subscriberTypes)
    {
        var interfaceType = subscriberType.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>));
        
        builder.Services.AddScoped(interfaceType, subscriberType);
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
