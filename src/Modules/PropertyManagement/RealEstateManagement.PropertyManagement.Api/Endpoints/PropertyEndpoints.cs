using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MediatR;
using RealEstate.Property.Application.Commands.RegisterProperty;
using RealEstate.Property.Application.Commands.UpdateProperty;
using RealEstate.Property.Application.Commands.ReserveProperty;
using RealEstate.Property.Application.Commands.SellProperty;
using RealEstate.Property.Application.Commands.CancelReservation;
using RealEstate.Property.Application.Commands.WithdrawProperty;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstate.Property.Application.Interfaces;
using RealEstate.Property.Application.DTOs;
using RealEstate.Property.Application.Mappers;

namespace RealEstate.Property.API.Endpoints;

public record ReservePropertyRequest(Guid OwnerId);

public static class PropertyEndpoints
{
    public static void MapPropertyEndpoints(this WebApplication app)
    {
        // CREATE / POST
        app.MapPost("/properties", async (
            RegisterPropertyCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var propertyId = await mediator.Send(command, ct);
                return Results.Created($"/properties/{propertyId}", new { id = propertyId });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return Results.Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        // READ / GET all (DTOs) with optional filters
        app.MapGet("/properties", async (
            IPropertyRepository repo,
            string? status,
            decimal? minPrice,
            decimal? maxPrice,
            CancellationToken ct) =>
        {
            IEnumerable<Domain.Aggregates.Property> properties;

             PropertyStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PropertyStatus>(status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }

            // Apply filters
            if (statusEnum.HasValue)
            {
                properties = await repo.GetByStatusAsync(statusEnum, ct);
            }
            else
            {
                properties = await repo.GetAllAsync(ct);
            }

             if (minPrice.HasValue || maxPrice.HasValue)
            {
                properties = properties.Where(p => 
                    (!minPrice.HasValue || p.Price.Amount >= minPrice.Value) &&
                    (!maxPrice.HasValue || p.Price.Amount <= maxPrice.Value));
            }

            var result = properties.Select(PropertyMapper.ToDto);
            return Results.Ok(result);
        });

        // READ / GET by Id (DTO)
        app.MapGet("/properties/{id}", async (
            Guid id,
            IPropertyRepository repo,
            CancellationToken ct) =>
        {
            var property = await repo.GetByIdAsync(id, ct);

            if (property is null)
                return Results.NotFound();

            return Results.Ok(PropertyMapper.ToDto(property));
        });

        // UPDATE / PUT 
         app.MapPut("/properties/{id}", async (
            Guid id,
            UpdatePropertyRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
 
                var command = new UpdatePropertyCommand(
                    id,  
                    request.City,
                    request.SubCity,
                    request.Street,
                    request.ZipCode,
                    request.PriceAmount,
                    request.Currency,
                    request.SizeSqMeters,
                    request.Bedrooms,
                    request.Bathrooms,
                    request.YearBuilt
                );
                
                await mediator.Send(command, ct);
                return Results.Ok();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Cannot update"))
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        // UPDATE / PATCH  
        app.MapPatch("/properties/{id}", async (
            Guid id,
            UpdatePropertyRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                 var command = new UpdatePropertyCommand(
                    id,   
                    request.City,
                    request.SubCity,
                    request.Street,
                    request.ZipCode,
                    request.PriceAmount,
                    request.Currency,
                    request.SizeSqMeters,
                    request.Bedrooms,
                    request.Bathrooms,
                    request.YearBuilt
                );
                
                await mediator.Send(command, ct);
                return Results.Ok();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Cannot update"))
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        // RESERVE / POST
        app.MapPost("/properties/{id}/reserve", async (
            Guid id,
            ReservePropertyRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var command = new ReservePropertyCommand(id, request.OwnerId);
                await mediator.Send(command, ct);
                return Results.Ok();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (
                ex.Message.Contains("cannot be reserved", StringComparison.OrdinalIgnoreCase) || 
                ex.Message.Contains("Cannot reserve", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("already reserved", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        // SELL / POST
        app.MapPost("/properties/{id}/sell", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var command = new SellPropertyCommand(id);
                await mediator.Send(command, ct);
                return Results.Ok();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already sold") || ex.Message.Contains("can only be sold"))
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        // CANCEL RESERVATION / POST
        app.MapPost("/properties/{id}/cancel-reservation", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var command = new CancelReservationCommand(id);
                await mediator.Send(command, ct);
                return Results.Ok();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("must be reserved") || ex.Message.Contains("cannot cancel"))
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        // WITHDRAW / POST
        app.MapPost("/properties/{id}/withdraw", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var command = new WithdrawPropertyCommand(id);
                await mediator.Send(command, ct);
                return Results.Ok();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already withdrawn") || ex.Message.Contains("Cannot withdraw"))
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });
    }
}

