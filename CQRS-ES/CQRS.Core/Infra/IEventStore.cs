﻿using CQRS.Core.Events;

namespace CQRS.Core.Infra;

public interface IEventStore
{
    Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
    Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
    Task<List<Guid>> GetAggregateIdsAsync();
} 