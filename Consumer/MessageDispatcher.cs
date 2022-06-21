using Consumer.Handlers;
using Contracts.Messages;

namespace Consumer;

public class MessageDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly Dictionary<string, Type> _messageMappings = new()
    {
        { nameof(CustomerCreated), typeof(CustomerCreated) },
        { nameof(CustomerDeleted), typeof(CustomerDeleted) },
    }; 

    private readonly Dictionary<string, Func<IServiceProvider, IMessageHandler>> _handlers = new()
    {
        { nameof(CustomerCreated), provider => provider.GetRequiredService<CustomerCreatedHandler>() },
        { nameof(CustomerDeleted), provider => provider.GetRequiredService<CustomerDeletedHandler>() },
    };

    public MessageDispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task DispatchAsync<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        using var scope = _scopeFactory.CreateScope();
        var handler = _handlers[message.MessageTypeName](scope.ServiceProvider);
        await handler.HandleAsync(message);
    }

    public bool CanHandleMessageType(string messageTypeName)
    {
        return _handlers.ContainsKey(messageTypeName);
    }

    public Type? GetMessageTypeByName(string messageTypeName)
    {
        return _messageMappings.GetValueOrDefault(messageTypeName);
    }
}