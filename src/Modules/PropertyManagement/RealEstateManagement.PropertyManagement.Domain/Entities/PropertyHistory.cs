using RealEstateManagement.Property.Domain.ValueObjects;

namespace RealEstateManagement.Property.Domain.Entities;

public sealed class PropertyHistory
{
    public HistoryId Id { get; private set; }
    public string ChangeType { get; private set; }
    public string OldValue { get; private set; }
    public string NewValue { get; private set; }
    public string ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; }

 
    private PropertyHistory()
    {
    }

    public PropertyHistory(
        HistoryId id,
        string changeType,
        string oldValue,
        string newValue,
        string changedBy)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        ChangeType = changeType ?? throw new ArgumentNullException(nameof(changeType));
        OldValue = oldValue ?? throw new ArgumentNullException(nameof(oldValue));
        NewValue = newValue ?? throw new ArgumentNullException(nameof(newValue));
        ChangedBy = changedBy ?? throw new ArgumentNullException(nameof(changedBy));
        ChangedAt = DateTime.UtcNow;
    }
}


