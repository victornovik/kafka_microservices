using Post.Query.Api;
using Post.Query.Infra.Consumers;
using Post.Query.Infra.Handlers;
using Post.Query.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddScoped<IPostRepository, PostRepository>();
services.AddScoped<ICommentRepository, CommentRepository>();
services.AddScoped<IEventHandler, Post.Query.Infra.Handlers.EventHandler>();

services.AddSqlDatabase(builder);
services.AddKafka(builder);
services.AddQueryHandlers();
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