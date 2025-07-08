using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;

namespace Post.Cmd.Infra.Producers;

public class EventProducer(IOptions<ProducerConfig> config) : IEventProducer
{
    public async Task ProduceAsync<T>(string topic, T evt) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string,string>(config.Value)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(), 
            Value = JsonSerializer.Serialize(evt, evt.GetType())
        };

        var res = await producer.ProduceAsync(topic, eventMessage);

        if (res.Status == PersistenceStatus.NotPersisted)
            throw new Exception($"Cannot produce `{evt.GetType()}` message to topic `{topic}` due to: {res.Message}");
    }
}