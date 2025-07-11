using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IEventStoreRepository
{
    Task SaveAsync(EventModel evt);
    Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId);
    Task<List<EventModel>> FindAllAsync();
}