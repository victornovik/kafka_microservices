# ASP.NET Core Microservices with Kafka

## Docker
```
docker network create --attachable -d bridge kafka_microservices_network
docker network ls

docker-compose up -d
```

## Create project, build, run
```
dotnet new classlib -o CQRS.Core
dotnet new classlib -o Post.Common

dotnet new webapi -o Post.Cmd.Api
dotnet new classlib -o Post.Cmd.Domain
dotnet new classlib -o Post.Cmd.Infra

dotnet new webapi -o Post.Query.Api
dotnet new classlib -o Post.Query.Domain
dotnet new classlib -o Post.Query.Infra

dotnet new sln
dotnet sln add CQRS-ES/CQRS.Core/CQRS.Core.csproj
dotnet sln add SM-Post/Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj
dotnet sln add SM-Post/Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj
dotnet sln add SM-Post/Post.Cmd/Post.Cmd.Infra/Post.Cmd.Infra.csproj

dotnet add SM-Post/Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj reference CQRS-ES/CQRS.Core/CQRS.Core.csproj

dotnet dev-certs https --trust

dotnet build
dotnet run --launch-profile https
```