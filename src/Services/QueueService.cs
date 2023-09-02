using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Exceptions;

public class QueueService : IQueueService
{
    ServiceBusClient serviceBusClient;
    ManagementClient managementClient;
    IReadOnlyList<string> queueNames = new List<string>();
    IDictionary<string, ServiceBusReceiver> receiverLookUp =
        new Dictionary<string, ServiceBusReceiver>();
    IDictionary<string, ServiceBusSender> senderLookUp = new Dictionary<string, ServiceBusSender>();

    public QueueService(string serviceBusConnectionString)
    {
        this.serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
        this.managementClient = new ManagementClient(serviceBusConnectionString);
    }

    public async Task Setup()
    {
        queueNames = await RetrieveQueueNames();
        receiverLookUp = CreateReceivers(queueNames);
        senderLookUp = CreateSenders(queueNames);
    }

    public IReadOnlyList<string> GetQueueNames() => queueNames;

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(
        string queueName,
        int noOfMessages
    )
    {
        var receiver = GetReceiverOrThrow(queueName);
        return await receiver.PeekMessagesAsync(noOfMessages);
    }

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekDeadLetterMessages(
        string queueName,
        int noOfMessages
    )
    {
        var receiver = GetDeadLetterReceiverOrThrow(queueName);
        return await receiver.PeekMessagesAsync(noOfMessages);
    }

    public async Task ResubmitDeadLetterMessages(
        string queueName,
        int fetchCount,
        TimeSpan? maxWaitTime = null
    )
    {
        maxWaitTime ??= new TimeSpan(0, 0, 10);

        var receiver = GetDeadLetterReceiverOrThrow(queueName);
        var sender = GetSenderOrThrow(queueName);

        var deadLetterMessages = await receiver.ReceiveMessagesAsync(fetchCount, maxWaitTime);

        foreach (var message in deadLetterMessages)
        {
            var resubmittableMessage = new ServiceBusMessage(message);
            await sender.SendMessageAsync(resubmittableMessage);
            await receiver.CompleteMessageAsync(message);
        }

        await receiver.CloseAsync();
    }

    private ServiceBusSender GetSenderOrThrow(string queueName)
    {
        if (!senderLookUp.TryGetValue(queueName, out var sender))
        {
            throw new NotFoundException(
                $"No ServiceBusSender was found for queue named '{queueName}'."
            );
        }

        return sender;
    }

    private ServiceBusReceiver GetReceiverOrThrow(string queueName)
    {
        if (!receiverLookUp.TryGetValue(queueName, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for queue named '{queueName}'."
            );
        }

        return receiver;
    }

    private ServiceBusReceiver GetDeadLetterReceiverOrThrow(string queueName)
    {
        var deadLetterQueueName = $"{queueName}/$deadletterqueue";
        if (!receiverLookUp.TryGetValue(deadLetterQueueName, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for queue named '{deadLetterQueueName}'."
            );
        }

        return receiver;
    }

    private async Task<IReadOnlyList<string>> RetrieveQueueNames()
    {
        var queues = await managementClient.GetQueuesAsync();
        return queues.Select(q => q.Path).ToList();
    }

    private IDictionary<string, ServiceBusReceiver> CreateReceivers(IReadOnlyList<string> queues)
    {
        var receiverLookUp = new Dictionary<string, ServiceBusReceiver>();

        foreach (var queue in queues)
        {
            var options = new ServiceBusReceiverOptions();

            var receiver = serviceBusClient.CreateReceiver(queue, options);
            receiverLookUp.Add(queue, receiver);

            var deadLetterQueue = $"{queue}/$deadletterqueue";
            var deadLetterRecevier = serviceBusClient.CreateReceiver(deadLetterQueue, options);
            receiverLookUp.Add(deadLetterQueue, deadLetterRecevier);
        }

        return receiverLookUp;
    }

    private IDictionary<string, ServiceBusSender> CreateSenders(IReadOnlyList<string> queues)
    {
        var senderLookUp = new Dictionary<string, ServiceBusSender>();

        foreach (var queue in queues)
        {
            var sender = serviceBusClient.CreateSender(queue);
            senderLookUp.Add(queue, sender);
        }

        return senderLookUp;
    }
}
