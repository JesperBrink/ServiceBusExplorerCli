using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Exceptions;

public class TopSubService : ITopSubService
{
    ServiceBusClient serviceBusClient;
    ManagementClient managementClient;
    IDictionary<string, IReadOnlyList<string>> topicToSubscriptions =
        new Dictionary<string, IReadOnlyList<string>>();
    IDictionary<string, ServiceBusReceiver> receiverLookUp =
        new Dictionary<string, ServiceBusReceiver>();
    IDictionary<string, ServiceBusSender> senderLookUp = new Dictionary<string, ServiceBusSender>();

    public TopSubService(string serviceBusConnectionString)
    {
        this.serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
        this.managementClient = new ManagementClient(serviceBusConnectionString);
    }

    public async Task Setup()
    {
        topicToSubscriptions = await RetrieveTopicAndSubscriptionNames();
        receiverLookUp = CreateReceivers(topicToSubscriptions);
        senderLookUp = CreateSenders(topicToSubscriptions);
    }

    public IReadOnlyList<string> GetTopicsAndSubscriptionNames()
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(
        string topicName,
        string subscriptionName,
        int noOfMessages
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekDeadLetterMessages(
        string topicName,
        string subscriptionName,
        int noOfMessages
    )
    {
        throw new NotImplementedException();
    }

    public Task ResubmitDeadLetterMessages(
        string topicName,
        string subscriptionName,
        int fetchCount,
        TimeSpan? maxWaitTime = null
    )
    {
        throw new NotImplementedException();
    }

    private async Task<
        IDictionary<string, IReadOnlyList<string>>
    > RetrieveTopicAndSubscriptionNames()
    {
        var topicToSubscriptions = new Dictionary<string, IReadOnlyList<string>>();

        var topics = await managementClient.GetTopicsAsync();
        var topicNames = topics.Select(t => t.Path).ToList();

        foreach (var topicName in topicNames)
        {
            var subscriptions = await managementClient.GetSubscriptionsAsync(topicName);
            var subscriptionNames = subscriptions.Select(s => s.SubscriptionName).ToList();
            topicToSubscriptions[topicName] = subscriptionNames;
        }

        return topicToSubscriptions;
    }

    private IDictionary<string, ServiceBusReceiver> CreateReceivers(
        IDictionary<string, IReadOnlyList<string>> topicToSubscriptions
    )
    {
        var receiverLookUp = new Dictionary<string, ServiceBusReceiver>();

        foreach (var topicName in topicToSubscriptions.Keys)
        {
            foreach (var subscriptionName in topicToSubscriptions[topicName])
            {
                var receiver = serviceBusClient.CreateReceiver(topicName, subscriptionName);
                var topicSubscriptionPath = GetTopicSubscriptionPath(topicName, subscriptionName);
                receiverLookUp.Add(topicSubscriptionPath, receiver);

                var topicSubscriptionDeadLetterPath = $"{topicSubscriptionPath}/$deadletterqueue";
                var deadLetterRecevier = serviceBusClient.CreateReceiver(
                    topicSubscriptionDeadLetterPath
                );
                receiverLookUp.Add(topicSubscriptionDeadLetterPath, deadLetterRecevier);
            }
        }

        return receiverLookUp;
    }

    private IDictionary<string, ServiceBusSender> CreateSenders(
        IDictionary<string, IReadOnlyList<string>> topicToSubscriptions
    )
    {
        var senderLookUp = new Dictionary<string, ServiceBusSender>();

        foreach (var topicName in topicToSubscriptions.Keys)
        {
            var sender = serviceBusClient.CreateSender(topicName);
            senderLookUp.Add(topicName, sender);
        }

        return senderLookUp;
    }

    private string GetTopicSubscriptionPath(string topicName, string subscriptionName)
    {
        return $"{topicName}/Subscriptions/{subscriptionName}";
    }
}
