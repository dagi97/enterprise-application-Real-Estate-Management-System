using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalesLoan.Application.Commands;
using SalesLoan.Application.Queries;
using SalesLoan.Domain.Repositories;
using SalesLoan.Infrastructure.Data;
using SalesLoan.Infrastructure.Repositories;
using Shared.Infrastructure.Outbox;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Sales & Loan API", Version = "v1" });
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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateLoanApplicationCommand).Assembly));

// Entity Framework - Shared database with schema separation
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Server=localhost,1433;Database=RealEstateManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

builder.Services.AddDbContext<SalesLoanDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<OutboxDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

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
var loans = app.MapGroup("/api/loans").RequireAuthorization();
var sales = app.MapGroup("/api/sales").RequireAuthorization();

loans.MapPost("/", async (CreateLoanApplicationCommand command, IMediator mediator) =>
{
    var loanId = await mediator.Send(command);
    return Results.Created($"/api/loans/{loanId}", new { Id = loanId });
})
.WithName("CreateLoanApplication")
.WithOpenApi();

loans.MapPost("/{id:guid}/approve", async (Guid id, ApproveLoanCommand command, IMediator mediator) =>
{
    if (id != command.LoanApplicationId)
        return Results.BadRequest();

    await mediator.Send(command);
    return Results.NoContent();
})
.WithName("ApproveLoan")
.WithOpenApi();

loans.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var loan = await mediator.Send(new GetLoanApplicationQuery(id));
    return loan == null ? Results.NotFound() : Results.Ok(loan);
})
.WithName("GetLoanApplication")
.WithOpenApi();

sales.MapPost("/", async (CreateSaleCommand command, IMediator mediator) =>
{
    var saleId = await mediator.Send(command);
    return Results.Created($"/api/sales/{saleId}", new { Id = saleId });
})
.WithName("CreateSale")
.WithOpenApi();

app.Run();
