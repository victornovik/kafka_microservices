using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IEventSourcingHandler
{
    Task SaveAsync(EventModel evt);
    Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId);
}