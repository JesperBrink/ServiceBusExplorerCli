using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

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
        }

        return receiverLookUp;
    }
}
