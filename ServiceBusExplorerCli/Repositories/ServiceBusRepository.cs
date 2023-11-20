using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using ServiceBusExplorerCli.Repositories.Interface;

namespace ServiceBusExplorerCli.Repositories;

public class ServiceBusRepository : IServiceBusRepository
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ManagementClient _managementClient;

    public ServiceBusRepository(
        ServiceBusClient serviceBusClient,
        ManagementClient managementClient
    )
    {
        _serviceBusClient = serviceBusClient;
        _managementClient = managementClient;
    }

    public ServiceBusReceiver CreateReceiver(string queueName)
    {
        return _serviceBusClient.CreateReceiver(queueName);
    }

    public ServiceBusReceiver CreateReceiver(string topicName, string subscriptionName)
    {
        return _serviceBusClient.CreateReceiver(topicName, subscriptionName);
    }

    public ServiceBusSender CreateSender(string queueOrTopicName)
    {
        return _serviceBusClient.CreateSender(queueOrTopicName);
    }

    public Task<IList<TopicDescription>> GetTopicsAsync()
    {
        return _managementClient.GetTopicsAsync();
    }

    public Task<IList<SubscriptionDescription>> GetSubscriptionsAsync(string topicPath)
    {
        return _managementClient.GetSubscriptionsAsync(topicPath);
    }

    public Task<IList<QueueDescription>> GetQueuesAsync()
    {
        return _managementClient.GetQueuesAsync();
    }
}
