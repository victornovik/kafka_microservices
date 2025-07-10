using CQRS.Core.Mediator;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Infra.Mediator;

public class QueryDispatcher : IQueryDispatcher<PostEntity>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> handlers = new();

    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
    {
        if (handlers.ContainsKey(typeof(TQuery)))
            throw new ArgumentException("Query handler is already registered");

        handlers.Add(typeof(TQuery), q => handler((TQuery)q));
    }

    public async Task<List<PostEntity>> SendAsync(BaseQuery query)
    {
        if (handlers.TryGetValue(query.GetType(), out var handler))
            return await handler(query);
        
        throw new ArgumentException("No query handler is registered");
    }
}