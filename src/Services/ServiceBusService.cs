using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Azure.Messaging.ServiceBus;
using Exceptions;

public class ServiceBusService : IServiceBusService
{
    ServiceBusClient serviceBusClient;
    ManagementClient managementClient;
    IReadOnlyList<string> queueNames = new List<string>();
    IDictionary<string, ServiceBusReceiver> receiverLookUp =
        new Dictionary<string, ServiceBusReceiver>();

    public ServiceBusService(string serviceBusConnectionString)
    {
        this.serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
        this.managementClient = new ManagementClient(serviceBusConnectionString);
    }

    public async Task Setup()
    {
        queueNames = await RetrieveQueueNames();
        receiverLookUp = await CreateReceivers(queueNames);
    }

    public IReadOnlyList<string> GetQueueNames() => queueNames;

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessagesInQueue(
        string queueName,
        int noOfMessages
    )
    {
        if (!receiverLookUp.TryGetValue(queueName, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for queue named '{queueName}'."
            );
        }

        return await receiver.PeekMessagesAsync(noOfMessages);
    }

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekDeadLetterMessagesInQueue(
        string queueName,
        int noOfMessages
    )
    {
        var deadLetterQueueName = $"{queueName}/$deadletterqueue";
        if (!receiverLookUp.TryGetValue(deadLetterQueueName, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for queue named '{deadLetterQueueName}'."
            );
        }

        return await receiver.PeekMessagesAsync(noOfMessages);
    } 

    private async Task<IReadOnlyList<string>> RetrieveQueueNames()
    {
        var queues = await managementClient.GetQueuesAsync();
        return queues.Select(q => q.Path).ToList();
    }

    private async Task<IDictionary<string, ServiceBusReceiver>> CreateReceivers(
        IReadOnlyList<string> queues
    )
    {
        var receiverLookUp = new Dictionary<string, ServiceBusReceiver>();

        foreach (var queue in queues)
        {
            var options = new ServiceBusReceiverOptions();

            var recevier = serviceBusClient.CreateReceiver(queue, options);
            receiverLookUp.Add(queue, recevier);

            var deadLetterQueue = $"{queue}/$deadletterqueue";
            var deadLetterRecevier = serviceBusClient.CreateReceiver(deadLetterQueue, options);
            receiverLookUp.Add(deadLetterQueue, deadLetterRecevier);
        }

        return receiverLookUp;
    }
}
