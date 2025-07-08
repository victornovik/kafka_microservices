using Microsoft.EntityFrameworkCore;
using Post.Query.Infra.DataAccess;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var connString = builder.Configuration.GetConnectionString("SqlServer");
Action<DbContextOptionsBuilder> configureAction = bld => bld.UseLazyLoadingProxies().UseSqlServer(connString);
services.AddDbContext<DatabaseContext>(configureAction);
services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureAction));

// Create database and tables from code
var dataContext = services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();

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
