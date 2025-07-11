using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Mediator;
using CQRS.Core.Producers;
using MongoDB.Bson.Serialization;
using Post.Cmd.Api.Commands;
using Post.Cmd.Infra.Config;
using Post.Cmd.Infra.Mediator;
using Post.Cmd.Infra.Producers;
using Post.Cmd.Infra.Repositories;
using Post.Common.Events;

namespace Post.Cmd.Api;

public static class Extensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler, CommandHandler>();

        var handler = services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
        var dispatcher = new CommandDispatcher();
        dispatcher.RegisterHandler<AddCommentCommand>(handler.HandleAsync);
        dispatcher.RegisterHandler<EditCommentCommand>(handler.HandleAsync);
        dispatcher.RegisterHandler<RemoveCommentCommand>(handler.HandleAsync);
        dispatcher.RegisterHandler<NewPostCommand>(handler.HandleAsync);
        dispatcher.RegisterHandler<EditPostCommand>(handler.HandleAsync);
        dispatcher.RegisterHandler<LikePostCommand>(handler.HandleAsync);
        dispatcher.RegisterHandler<DeletePostCommand>(handler.HandleAsync);
        dispatcher.RegisterHandler<RestoreReadDbCommand>(handler.HandleAsync);

        services.AddSingleton<ICommandDispatcher>(_ => dispatcher);

        return services;
    }

    public static IServiceCollection AddMongo(this IServiceCollection services, WebApplicationBuilder builder)
    {
        BsonClassMap.RegisterClassMap<BaseEvent>();
        BsonClassMap.RegisterClassMap<PostCreatedEvent>();
        BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
        BsonClassMap.RegisterClassMap<PostLikedEvent>();
        BsonClassMap.RegisterClassMap<CommentAddedEvent>();
        BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
        BsonClassMap.RegisterClassMap<CommentRemovedEvent>();
        BsonClassMap.RegisterClassMap<PostRemovedEvent>();

        services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
        services.AddScoped<IEventStoreRepository, MongoRepository>();

        return services;
    }

    public static IServiceCollection AddKafka(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
        services.AddScoped<IEventProducer, KafkaProducer>();

        return services;
    }
}