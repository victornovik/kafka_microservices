using CQRS.Core.Commands;
using CQRS.Core.Mediator;

namespace Post.Cmd.Infra.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> handlers = new();

    public void RegisterHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : BaseCommand
    {
        if (handlers.ContainsKey(typeof(TCommand)))
            throw new ArgumentException("You cannot register the same command handler twice");

        handlers.Add(typeof(TCommand), baseCommand => handler((TCommand)baseCommand));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (!handlers.TryGetValue(command.GetType(), out var handler))
            throw new ArgumentException($"No command handler was registered for {command.GetType()}");

        await handler(command);
    }
}