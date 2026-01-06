namespace RealEstate.Property.Domain.Aggregates;

using System;
using System.Collections.Generic;
using RealEstate.Property.Domain.Events;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstateManagement.Property.Domain.Entities;
using RealEstate.Shared.Domain.ValueObjects;

public sealed class Property
{
    public PropertyId Id { get; private set; }
    public Address Address { get; private set; }
    public PropertyStatus Status { get; private set; }
    public Money Price { get; private set; }
    public Owner? Owner { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

     private Property()
    {
     }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public Property(PropertyId id, Address address, Money price)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Status = PropertyStatus.Available;
        
        _domainEvents.Add(new PropertyRegistered(id, DateTime.UtcNow));
    }

    public void Reserve(Owner owner)
    {
        if (owner == null)
            throw new ArgumentNullException(nameof(owner));
        if (Status == PropertyStatus.Reserved)
            throw new InvalidOperationException("Property is already reserved.");
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Cannot reserve a sold property.");
        if (Status != PropertyStatus.Available)
            throw new InvalidOperationException("Property cannot be reserved unless available.");

        Status = PropertyStatus.Reserved;
        Owner = owner;
        _domainEvents.Add(new PropertyReserved(Id, DateTime.UtcNow));
    }

    public void MarkAsSold()
    {
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Property is already sold.");
        if (Status != PropertyStatus.Available && Status != PropertyStatus.Reserved)
            throw new InvalidOperationException("Property can only be sold if it is available or reserved.");

        var previousOwner = Owner;
        Status = PropertyStatus.Sold;
        Owner = null;
        
        _domainEvents.Add(new PropertySold(Id, DateTime.UtcNow));
    }

    public void CancelReservation()
    {
        if (Status != PropertyStatus.Reserved)
            throw new InvalidOperationException("Property must be reserved to cancel reservation.");

        Status = PropertyStatus.Available;
        Owner = null;
        _domainEvents.Add(new PropertyReservationCancelled(Id, DateTime.UtcNow));
    }

    public void UpdateAddress(Address newAddress)
    {
        if (newAddress == null)
            throw new ArgumentNullException(nameof(newAddress));
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Cannot update a sold property.");
        if (Status == PropertyStatus.Reserved)
            throw new InvalidOperationException("Cannot update a reserved property.");

        var oldAddress = Address;
        Address = newAddress;
        _domainEvents.Add(new PropertyUpdated(Id, oldAddress, newAddress, null, null, DateTime.UtcNow));
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice == null)
            throw new ArgumentNullException(nameof(newPrice));
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Cannot update a sold property.");
        if (Status == PropertyStatus.Reserved)
            throw new InvalidOperationException("Cannot update a reserved property.");

        var oldPrice = Price;
        Price = newPrice;
        _domainEvents.Add(new PropertyUpdated(Id, null, null, oldPrice, newPrice, DateTime.UtcNow));
    }

    public void Update(Address? newAddress, Money? newPrice)
    {
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Cannot update a sold property.");
        if (Status == PropertyStatus.Reserved)
            throw new InvalidOperationException("Cannot update a reserved property.");

        var oldAddress = Address;
        var oldPrice = Price;
        var addressChanged = false;
        var priceChanged = false;

        if (newAddress != null)
        {
            Address = newAddress;
            addressChanged = true;
        }

        if (newPrice != null)
        {
            Price = newPrice;
            priceChanged = true;
        }

        if (addressChanged || priceChanged)
        {
            _domainEvents.Add(new PropertyUpdated(
                Id,
                addressChanged ? oldAddress : null,
                addressChanged ? newAddress : null,
                priceChanged ? oldPrice : null,
                priceChanged ? newPrice : null,
                DateTime.UtcNow));
        }
    }
}
