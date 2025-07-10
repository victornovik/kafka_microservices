using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Mediator;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Infra.Consumers;
using Post.Query.Infra.Mediator;

namespace Post.Query.Api;

public static class Extensions
{
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler, QueryHandler>();

        var handler = services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
        var dispatcher = new QueryDispatcher();
        dispatcher.RegisterHandler<FindAllPostsQuery>(handler.HandleAsync);
        dispatcher.RegisterHandler<FindPostByIdQuery>(handler.HandleAsync);
        dispatcher.RegisterHandler<FindPostsByAuthorQuery>(handler.HandleAsync);
        dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(handler.HandleAsync);
        dispatcher.RegisterHandler<FindPostsWithLikesQuery>(handler.HandleAsync);

        services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispatcher);

        return services;
    }
    
    public static IServiceCollection AddKafka(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
        services.AddScoped<IEventConsumer, KafkaConsumer>();

        return services;
    }
}