using Amazon.SQS;
using Consumer;
using Consumer.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<SqsConsumerService>();
builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(Amazon.RegionEndpoint.EUWest2));
builder.Services.AddSingleton<MessageDispatcher>();
builder.Services.AddMessageHandlers();

var app = builder.Build();

app.Run();