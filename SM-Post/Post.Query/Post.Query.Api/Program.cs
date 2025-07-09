using Confluent.Kafka;
using CQRS.Core.Consumers;
using Microsoft.EntityFrameworkCore;
using Post.Query.Infra.Consumers;
using Post.Query.Infra.DataAccess;
using Post.Query.Infra.Handlers;
using Post.Query.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var connString = builder.Configuration.GetConnectionString("SqlServer");
Action<DbContextOptionsBuilder> configureAction = bld => bld.UseLazyLoadingProxies().UseSqlServer(connString);
services.AddDbContext<DatabaseContext>(configureAction);
services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureAction));

// Create database and tables from code
var dataContext = services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();

services.AddScoped<IPostRepository, PostRepository>();
services.AddScoped<ICommentRepository, CommentRepository>();
services.AddScoped<IEventHandler, Post.Query.Infra.Handlers.EventHandler>();

// Kafka configuration
services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
services.AddScoped<IEventConsumer, EventConsumer>();

services.AddControllers();
services.AddHostedService<ConsumerHostedService>();
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
