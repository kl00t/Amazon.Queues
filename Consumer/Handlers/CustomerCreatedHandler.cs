using Contracts.Messages;

namespace Consumer.Handlers;

public class CustomerCreatedHandler : IMessageHandler
{
    private readonly ILogger<CustomerCreated> _logger;

    public CustomerCreatedHandler(ILogger<CustomerCreated> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IMessage message)
    {
        var customerCreated = (CustomerCreated)message;
        _logger.LogInformation("Customer created with name: {FullName}", customerCreated.FullName);
        return Task.CompletedTask;
    }

    public static Type MessageType { get; } = typeof(CustomerCreated);
}