using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infra;
using CQRS.Core.Mediator;
using CQRS.Core.Producers;
using MongoDB.Bson.Serialization;
using Post.Cmd.Api;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infra.Config;
using Post.Cmd.Infra.Handlers;
using Post.Cmd.Infra.Mediator;
using Post.Cmd.Infra.Producers;
using Post.Cmd.Infra.Repositories;
using Post.Cmd.Infra.Stores;
using Post.Common.Events;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddMongo(builder);
services.AddKafka(builder);
services.AddScoped<IEventStore, EventStore>();
services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
services.AddCommandHandlers();
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