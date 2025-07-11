using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infra;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using static CQRS.Core.Events.BaseEvent;

namespace Post.Cmd.Infra.Stores;

public class EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer) : IEventStore
{
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await eventStoreRepository.FindByAggregateIdAsync(aggregateId);
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

            await eventStoreRepository.SaveAsync(eventModel);

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await eventProducer.ProduceAsync(topic, evt);
        }
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await eventStoreRepository.FindByAggregateIdAsync(aggregateId);
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException($"Incorrect post ID provided: {aggregateId}");

        return eventStream
            .OrderBy(x => x.Version)
            .Select(x => x.EventData)
            .ToList();
    }

    public async Task<List<Guid>> GetAggregateIdsAsync()
    {
        var eventStream = await eventStoreRepository.FindAllAsync();
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException("Cannot retrieve events");

        return eventStream
            .Select(x => x.AggregateId)
            .Distinct()
            .ToList();
    }
}