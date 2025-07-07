using CQRS.Core.Events;
using static CQRS.Core.Events.BaseEvent;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    private readonly List<BaseEvent> changes = [];

    public Guid AggregateId { get; protected set; }
    public int Version { get; set; } = DefaultEventVersion;

    public IEnumerable<BaseEvent> GetUncommitedChanges() => changes;

    public void MarkChangesAsCommited() => changes.Clear();

    private void ApplyChange(BaseEvent evt, bool isNew)
    {
        var method = GetType().GetMethod("Apply", [evt.GetType()]);
        ArgumentNullException.ThrowIfNull(method);
        method.Invoke(this, [evt]);

        if (isNew)
        {
            changes.Add(evt);
        }
    }

    protected void RaiseEvent(BaseEvent evt)
    {
        ApplyChange(evt, isNew: true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var e in events)
        {
            ApplyChange(e, isNew: false);
        }
    }
}