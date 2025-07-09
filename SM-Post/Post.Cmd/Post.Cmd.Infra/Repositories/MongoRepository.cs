using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Post.Cmd.Infra.Config;

namespace Post.Cmd.Infra.Repositories;

public class MongoRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> eventStoreCollection;

    static MongoRepository()
    {
        // Save GUID and DateTimeOffset as string
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    }

    public MongoRepository(IOptions<MongoDbConfig> cfg)
    {
        var mongoClient = new MongoClient(cfg.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(cfg.Value.Database);
        eventStoreCollection = mongoDatabase.GetCollection<EventModel>(cfg.Value.Collection);
    }

    public async Task SaveAsync(EventModel evt)
    {
        await eventStoreCollection.InsertOneAsync(evt).ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId)
    {
        return await eventStoreCollection
            .Find(filter => filter.AggregateId == aggregateId)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}