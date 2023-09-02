using Azure.Messaging.ServiceBus;

public interface ITopSubService
{
    public Task Setup();
    public IReadOnlyList<string> GetTopicsAndSubscriptionNames();
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
