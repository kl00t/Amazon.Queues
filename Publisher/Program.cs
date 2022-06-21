using Amazon.SQS;
using Publisher;
using Contracts.Messages;

var sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.EUWest2);

var publisher = new SqsPublisher(sqsClient);

await publisher.PublishAsync("customers", new CustomerCreated
{
    Id = 1,
    FullName = "Scott Vaughan"
});

await Task.Delay(5000);

await publisher.PublishAsync("customers", new CustomerDeleted
{
    Id = 1
});