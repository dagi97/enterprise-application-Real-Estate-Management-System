using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PricingValuation.Application.Commands;
using PricingValuation.Domain.DomainServices;
using PricingValuation.Domain.Repositories;
using PricingValuation.Infrastructure.Data;
using PricingValuation.Infrastructure.DomainServices;
using PricingValuation.Infrastructure.Repositories;
using Shared.Infrastructure.Outbox;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Pricing & Valuation API", Version = "v1" });
});

// Keycloak Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"] ?? "http://localhost:8080/realms/master";
        options.Audience = builder.Configuration["Keycloak:Audience"] ?? "account";
        options.RequireHttpsMetadata = false;
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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EstimatePriceCommand).Assembly));

// Entity Framework
builder.Services.AddDbContext<PricingValuationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Server=localhost,1433;Database=PricingValuation;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"));

builder.Services.AddDbContext<OutboxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Server=localhost,1433;Database=PricingValuation;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"));

// Repositories
builder.Services.AddScoped<IPriceEstimateRepository, PriceEstimateRepository>();
builder.Services.AddScoped<IPriceEstimationService, PriceEstimationService>();

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
var pricing = app.MapGroup("/api/pricing").RequireAuthorization();

pricing.MapPost("/estimate", async (EstimatePriceCommand command, IMediator mediator) =>
{
    var estimatedPrice = await mediator.Send(command);
    return Results.Ok(new { EstimatedPrice = estimatedPrice });
})
.WithName("EstimatePrice")
.WithOpenApi();

app.Run();
