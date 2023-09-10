using Azure.Messaging.ServiceBus;

namespace ServiceBusExplorerCli.Services.Interface;

public interface IPubSubService
{
    public Task Setup();
    public IDictionary<string, IReadOnlyList<string>> GetTopicsAndSubscriptionNames();
    public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(
        string topicName,
        string subscriptionName,
        int noOfMessages
    );
    public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekDeadLetterMessages(
        string topicName,
        string subscriptionName,
        int noOfMessages
    );
    public Task ResubmitDeadLetterMessages(
        string topicName,
        string subscriptionName,
        int fetchCount,
        TimeSpan? maxWaitTime = null
    );
}
