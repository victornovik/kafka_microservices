using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infra;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infra.Handlers;

public class EventSourcingHandler(IEventStore EventStore) : IEventSourcingHandler<PostAggregate>
{
    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await EventStore.SaveEventsAsync(aggregate.AggregateId, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommited();
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        
        var events = await EventStore.GetEventsAsync(aggregateId);
        if (events == null || !events.Any())
            return aggregate;

        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();

        return aggregate;
    }
}