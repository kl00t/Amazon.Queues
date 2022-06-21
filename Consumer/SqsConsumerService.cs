using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net;

namespace Consumer;

public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private const string QueueName = "customers";
    private readonly List<string> _messageAttributeNames = new() { "All" };

    public SqsConsumerService(IAmazonSQS sqs)
    {
        _sqs = sqs;
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
                Console.WriteLine(message.Body);
                await _sqs.DeleteMessageAsync(queueUrl.QueueUrl, message.ReceiptHandle, stoppingToken);
            }
        }
    }
}