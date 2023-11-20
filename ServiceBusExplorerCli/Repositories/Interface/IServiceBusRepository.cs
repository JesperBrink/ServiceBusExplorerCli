using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace ServiceBusExplorerCli.Repositories.Interface;

public interface IServiceBusRepository
{
    public ServiceBusReceiver CreateReceiver(string queueName);
    public ServiceBusReceiver CreateReceiver(string topicName, string subscriptionName);
    public ServiceBusSender CreateSender(string queueOrTopicName);
    public Task<IList<TopicDescription>> GetTopicsAsync();
    public Task<IList<SubscriptionDescription>> GetSubscriptionsAsync(string topicPath);
    public Task<IList<QueueDescription>> GetQueuesAsync();
}
