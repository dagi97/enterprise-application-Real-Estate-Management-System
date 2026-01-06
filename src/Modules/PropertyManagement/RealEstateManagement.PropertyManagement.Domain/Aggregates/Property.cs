namespace RealEstate.Property.Domain.Aggregates;

using System;
using System.Collections.Generic;
using System.Linq;
using RealEstate.Property.Domain.Events;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstateManagement.Property.Domain.Entities;
using RealEstate.Shared.Domain.ValueObjects;

public sealed class Property
{
    public PropertyId Id { get; private set; }
    public BranchId BranchId { get; private set; }
    public Address Address { get; private set; }
    public PropertyStatus Status { get; private set; }
    public Money Price { get; private set; }
    public OwnerId? OwnerId { get; private set; }
    public PropertyFeatures Features { get; private set; }

    private readonly List<PropertyHistory> _history = new();
    public IReadOnlyList<PropertyHistory> History => _history.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Parameterless constructor for EF Core
    private Property()
    {
        // EF Core requires a parameterless constructor
        // This should never be called directly in domain code
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public Property(
        PropertyId id,
        BranchId branchId,
        Address address,
        Money price,
        PropertyFeatures features)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        BranchId = branchId ?? throw new ArgumentNullException(nameof(branchId));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Features = features ?? throw new ArgumentNullException(nameof(features));
        Status = PropertyStatus.Available;
        OwnerId = null;

        _domainEvents.Add(new PropertyRegistered(Id, BranchId, Address, DateTime.UtcNow));
        AddHistory("PropertyRegistered", string.Empty, "Registered", "System");
    }

    public void Reserve(OwnerId ownerId)
    {
        if (ownerId == null)
            throw new ArgumentNullException(nameof(ownerId));
        if (Status == PropertyStatus.Reserved)
            throw new InvalidOperationException("Property is already reserved.");
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Cannot reserve a sold property.");
        if (Status == PropertyStatus.Withdrawn)
            throw new InvalidOperationException("Cannot reserve a withdrawn property.");
        if (Status != PropertyStatus.Available)
            throw new InvalidOperationException("Property cannot be reserved unless available.");

        var oldStatus = Status.ToString();
        Status = PropertyStatus.Reserved;
        OwnerId = ownerId;

        _domainEvents.Add(new PropertyReserved(Id, BranchId, ownerId, DateTime.UtcNow));
        AddHistory("StatusChanged", oldStatus, Status.ToString(), "System");
    }

    public void MarkAsSold()
    {
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Property is already sold.");
        if (Status != PropertyStatus.Available && Status != PropertyStatus.Reserved)
            throw new InvalidOperationException("Property can only be sold if it is available or reserved.");

        var oldStatus = Status.ToString();
        Status = PropertyStatus.Sold;
        var previousOwnerId = OwnerId;
        OwnerId = null;

        _domainEvents.Add(new PropertySold(Id, BranchId, DateTime.UtcNow));
        AddHistory("StatusChanged", oldStatus, Status.ToString(), "System");
        if (previousOwnerId != null)
        {
            AddHistory("OwnerCleared", previousOwnerId.Value.ToString(), string.Empty, "System");
        }
    }

    public void Withdraw()
    {
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Cannot withdraw a sold property.");
        if (Status == PropertyStatus.Withdrawn)
            throw new InvalidOperationException("Property is already withdrawn.");

        var oldStatus = Status.ToString();
        Status = PropertyStatus.Withdrawn;

        _domainEvents.Add(new PropertyWithdrawn(Id, BranchId, DateTime.UtcNow));
        AddHistory("StatusChanged", oldStatus, Status.ToString(), "System");
    }

    public void CancelReservation()
    {
        if (Status != PropertyStatus.Reserved)
            throw new InvalidOperationException("Property must be reserved to cancel reservation.");

        var oldStatus = Status.ToString();
        Status = PropertyStatus.Available;
        var previousOwnerId = OwnerId;
        OwnerId = null;

        _domainEvents.Add(new PropertyReservationCancelled(Id, BranchId, DateTime.UtcNow));
        AddHistory("StatusChanged", oldStatus, Status.ToString(), "System");
        if (previousOwnerId != null)
        {
            AddHistory("OwnerCleared", previousOwnerId.Value.ToString(), string.Empty, "System");
        }
    }

    public void UpdateDetails(Address? address, Money? price, PropertyFeatures? features)
    {
        if (Status == PropertyStatus.Sold)
            throw new InvalidOperationException("Cannot update a sold property.");
        if (Status == PropertyStatus.Reserved)
            throw new InvalidOperationException("Cannot update a reserved property.");
        if (Status == PropertyStatus.Withdrawn)
            throw new InvalidOperationException("Cannot update a withdrawn property.");

        var addressChanged = false;
        var priceChanged = false;
        var featuresChanged = false;

        Address? oldAddress = null;
        Money? oldPrice = null;
        PropertyFeatures? oldFeatures = null;

        if (address != null && !Address.Equals(address))
        {
            oldAddress = Address;
            Address = address;
            addressChanged = true;
            AddHistory("AddressChanged", oldAddress.ToString() ?? string.Empty, address.ToString() ?? string.Empty, "System");
        }

        if (price != null && !Price.Equals(price))
        {
            oldPrice = Price;
            Price = price;
            priceChanged = true;
            AddHistory("PriceChanged", oldPrice.Amount.ToString("F2"), price.Amount.ToString("F2"), "System");
        }

        if (features != null && !Features.Equals(features))
        {
            oldFeatures = Features;
            Features = features;
            featuresChanged = true;
            AddHistory("FeaturesChanged", oldFeatures.ToString() ?? string.Empty, features.ToString() ?? string.Empty, "System");
        }

        if (addressChanged || priceChanged || featuresChanged)
        {
            _domainEvents.Add(new PropertyUpdated(
                Id,
                BranchId,
                addressChanged ? oldAddress : null,
                addressChanged ? address : null,
                priceChanged ? oldPrice : null,
                priceChanged ? price : null,
                DateTime.UtcNow));
        }
    }

    private void AddHistory(string changeType, string oldValue, string newValue, string changedBy)
    {
        var historyId = new HistoryId(Guid.NewGuid());
        var history = new PropertyHistory(historyId, changeType, oldValue, newValue, changedBy);
        _history.Add(history);
    }
}
