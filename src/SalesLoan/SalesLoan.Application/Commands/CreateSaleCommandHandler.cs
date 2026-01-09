using MediatR;
using SalesLoan.Application.Commands;
using SalesLoan.Domain.Aggregates;
using SalesLoan.Domain.Repositories;
using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Application.Commands;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;

    public CreateSaleCommandHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var customer = new CustomerInfo(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Address);

        var saleAmount = new Money(request.SaleAmount, request.Currency);

        var sale = Sale.Create(
            request.PropertyId,
            request.BranchId,
            request.CustomerId,
            customer,
            saleAmount,
            request.Notes);

        sale.Confirm();
        sale.Complete();

        await _saleRepository.AddAsync(sale, cancellationToken);

        return sale.Id;
    }
}

