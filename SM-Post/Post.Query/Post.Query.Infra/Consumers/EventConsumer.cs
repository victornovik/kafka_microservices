using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Query.Infra.Converters;
using Post.Query.Infra.Handlers;
using System.Text.Json;

namespace Post.Query.Infra.Consumers;

public class EventConsumer(ILogger<EventConsumer> logger, IOptions<ConsumerConfig> config, IEventHandler eventHandler) : IEventConsumer
{
    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(config.Value)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);

        while (true)
        {
            try
            {
                var res = consumer.Consume();
                if (res?.Message == null)
                    continue;

                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var evt = JsonSerializer.Deserialize<BaseEvent>(res.Message.Value, options);

                var handler = eventHandler.GetType().GetMethod("On", [evt.GetType()]);
                ArgumentNullException.ThrowIfNull(handler);
                handler.Invoke(eventHandler, [evt]);

                // Increment Kafka offset
                consumer.Commit(res);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e, "Error while handling consumed event");
            }
        }
    }
}