using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PropertyManagement.Application.Commands;
using PropertyManagement.Application.Queries;
using PropertyManagement.Domain.Repositories;
using PropertyManagement.Infrastructure.Data;
using PropertyManagement.Infrastructure.Repositories;
using Shared.Infrastructure.Outbox;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Property Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Keycloak Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"] ?? "http://localhost:8080/realms/realestate-management";
        options.Audience = builder.Configuration["Keycloak:Audience"] ?? "account";
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterPropertyCommand).Assembly));

// Entity Framework - Shared database with schema separation
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Server=localhost,1433;Database=RealEstateManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

builder.Services.AddDbContext<PropertyManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<OutboxDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

// JSON Options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// API Endpoints
var properties = app.MapGroup("/api/properties").RequireAuthorization();

properties.MapPost("/", async (RegisterPropertyCommand command, IMediator mediator) =>
{
    var propertyId = await mediator.Send(command);
    return Results.Created($"/api/properties/{propertyId}", new { Id = propertyId });
})
.WithName("RegisterProperty")
.WithOpenApi();

properties.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var property = await mediator.Send(new GetPropertyQuery(id));
    return property == null ? Results.NotFound() : Results.Ok(property);
})
.WithName("GetProperty")
.WithOpenApi();

properties.MapPut("/{id:guid}/price", async (Guid id, UpdatePropertyPriceCommand command, IMediator mediator) =>
{
    if (id != command.PropertyId)
        return Results.BadRequest();

    await mediator.Send(command);
    return Results.NoContent();
})
.WithName("UpdatePropertyPrice")
.WithOpenApi();

app.Run();
