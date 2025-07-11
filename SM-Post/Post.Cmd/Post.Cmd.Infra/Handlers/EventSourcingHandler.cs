using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infra;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infra.Handlers;

public class EventSourcingHandler(IEventStore eventStore, IEventProducer producer) : IEventSourcingHandler<PostAggregate>
{
    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await eventStore.SaveEventsAsync(aggregate.AggregateId, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommited();
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        
        var events = await eventStore.GetEventsAsync(aggregateId);
        if (events == null || !events.Any())
            return aggregate;

        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();

        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await eventStore.GetAggregateIdsAsync();
        if (aggregateIds == null || !aggregateIds.Any())
            return;

        foreach (var aggregateId in aggregateIds)
        {
            var aggregate = await GetByIdAsync(aggregateId);
            if (aggregate == null || !aggregate.IsActive)
                continue;

            var events = await eventStore.GetEventsAsync(aggregateId);
            foreach (var evt in events)
            {
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await producer.ProduceAsync(topic, evt);
            }
        }
    }
}