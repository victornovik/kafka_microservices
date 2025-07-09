using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infra;
using CQRS.Core.Mediator;
using CQRS.Core.Producers;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infra.Config;
using Post.Cmd.Infra.Handlers;
using Post.Cmd.Infra.Mediator;
using Post.Cmd.Infra.Producers;
using Post.Cmd.Infra.Repositories;
using Post.Cmd.Infra.Stores;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Mongo configuration
services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
// Kafka configuration
services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

services.AddScoped<IEventStoreRepository, EventStoreRepository>();
services.AddScoped<IEventProducer, EventProducer>();
services.AddScoped<IEventStore, EventStore>();
services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
services.AddScoped<ICommandHandler, CommandHandler>();

// Register command handlers
var commandHandler = services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
var commandDispatcher = new CommandDispatcher();
commandDispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<EditPostCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
commandDispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
services.AddSingleton<ICommandDispatcher>(_ => commandDispatcher);

services.AddControllers();
services.AddOpenApi();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
