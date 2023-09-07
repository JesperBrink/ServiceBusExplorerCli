using Azure.Messaging.ServiceBus;

namespace ServiceBusExplorerCli.Services.Interface;

public interface IQueueService
{
    public Task Setup();
    public IReadOnlyList<string> GetQueueNames();
    public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(
        string queueName,
        int noOfMessages
    );
    public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekDeadLetterMessages(
        string queueName,
        int noOfMessages
    );
    public Task ResubmitDeadLetterMessages(
        string queueName,
        int fetchCount,
        TimeSpan? maxWaitTime = null
    );
}
