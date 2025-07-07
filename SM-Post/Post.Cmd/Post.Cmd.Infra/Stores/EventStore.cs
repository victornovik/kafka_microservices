using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infra;
using Post.Cmd.Domain.Aggregates;
using static CQRS.Core.Events.BaseEvent;

namespace Post.Cmd.Infra.Stores;

public class EventStore(IEventSourcingHandler EventStoreRepository) : IEventStore
{
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await EventStoreRepository.FindByAggregateIdAsync(aggregateId);
        if (expectedVersion != DefaultEventVersion && eventStream[^1].Version != expectedVersion)
            throw new ConcurrencyException();

        var version = expectedVersion;
        foreach (var evt in events)
        {
            version++;
            evt.Version = version;
            var eventType = evt.GetType().Name;
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.UtcNow, 
                AggregateId = aggregateId, 
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = evt
            };

            await EventStoreRepository.SaveAsync(eventModel);
        }
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await EventStoreRepository.FindByAggregateIdAsync(aggregateId);
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException($"Incorrect post ID provided: {aggregateId}");

        return eventStream
            .OrderBy(x => x.Version)
            .Select(x => x.EventData)
            .ToList();
    }
}