using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace Publisher;

public class SqsPublisher
{
    private readonly IAmazonSQS _sqs;

    public SqsPublisher(IAmazonSQS sqs)
    {
        _sqs = sqs;
    }

    public async Task PublishAsync<T>(string queueName, T message)
    {
        var queueUrl = await _sqs.GetQueueUrlAsync(queueName);
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageBody = JsonSerializer.Serialize(message)
        };

        await _sqs.SendMessageAsync(request);
    }
}