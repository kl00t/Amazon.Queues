using Amazon.SQS;
using Amazon.SQS.Model;
using Contracts.Messages;
using System.Net;
using System.Text.Json;

namespace Consumer;

public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly MessageDispatcher _dispatcher;
    private const string QueueName = "customers";
    private readonly List<string> _messageAttributeNames = new() { "All" };

    public SqsConsumerService(IAmazonSQS sqs, MessageDispatcher dispatcher)
    {
        _sqs = sqs;
        _dispatcher = dispatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = await _sqs.GetQueueUrlAsync(QueueName, stoppingToken);
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageAttributeNames = _messageAttributeNames,
            AttributeNames = _messageAttributeNames
        };

        while(!stoppingToken.IsCancellationRequested)
        {
            var messageResponse = await _sqs.ReceiveMessageAsync(receiveRequest, stoppingToken);
            if(messageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"HTTP Status Code returned {messageResponse.HttpStatusCode}");
            }

            foreach (var message in messageResponse.Messages)
            {
                var messageTypeName = message.MessageAttributes.GetValueOrDefault(nameof(IMessage.MessageTypeName))?.StringValue;
                if (messageTypeName is null)
                {
                    throw new ApplicationException("Message Type Name is null.");
                }

                var messageType = _dispatcher.GetMessageTypeByName(messageTypeName)!;
                var messageAsType = (IMessage)JsonSerializer.Deserialize(message.Body, messageType)!;

                await _dispatcher.DispatchAsync(messageAsType);
                await _sqs.DeleteMessageAsync(queueUrl.QueueUrl, message.ReceiptHandle, stoppingToken);
            }
        }
    }
}