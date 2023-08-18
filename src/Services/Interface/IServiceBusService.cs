using Azure.Messaging.ServiceBus;

public interface IServiceBusService
{
    public Task Setup();
    public IReadOnlyList<string> GetQueueNames();
    public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessagesInQueue(
        string queueName,
        int noOfMessages
    );
}
