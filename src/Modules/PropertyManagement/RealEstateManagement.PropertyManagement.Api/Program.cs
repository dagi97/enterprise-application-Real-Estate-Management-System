using MediatR;
using RealEstate.Property.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using RealEstate.Property.Application.Commands.RegisterProperty;
using RealEstate.Property.API.Endpoints;
using RealEstate.Property.Infrastructure.Persistence.Repositories;
using RealEstate.Property.Application.Interfaces;
using RealEstate.Property.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

 builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AllowTrailingCommas = true;
});

 builder.Services.AddDbContext<PropertyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PropertyDatabase")));

 builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterPropertyCommand).Assembly);
});

 builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

 builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

 // Outbox publisher and Quartz job registration will be added here once shared implementation is ready

 builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

 if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Property API V1");
    });
}

 app.MapPropertyEndpoints();

app.Run();
