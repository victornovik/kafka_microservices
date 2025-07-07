using CQRS.Core.Domain;
using CQRS.Core.Infra;
using Post.Cmd.Infra.Config;
using Post.Cmd.Infra.Repositories;
using Post.Cmd.Infra.Stores;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));

services.AddScoped<IEventSourcingHandler, EventStoreRepository>();
services.AddScoped<IEventStore, EventStore>();
services.AddControllers();
services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
