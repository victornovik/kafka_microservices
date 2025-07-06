using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    private readonly List<BaseEvent> changes = [];

    public Guid Id { get; protected set; }
    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommitedChanges() => changes;
    
    public void MarkChangesAsCommited() => changes.Clear();

    private void ApplyChange(BaseEvent @event, bool isNew)
    {
        var method = GetType().GetMethod("Apply", [@event.GetType()]);
        ArgumentNullException.ThrowIfNull(method);

        method.Invoke(this, [@event]);

        if (isNew)
            changes.Add(@event);
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, isNew: true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var e in events)
        {
            ApplyChange(e, isNew:false);
        }
    }
}