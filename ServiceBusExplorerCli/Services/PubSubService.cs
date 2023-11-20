using Azure.Messaging.ServiceBus;
using ServiceBusExplorerCli.Exceptions;
using ServiceBusExplorerCli.Repositories.Interface;
using ServiceBusExplorerCli.Services.Interface;

namespace ServiceBusExplorerCli.Services;

public class PubSubService : IPubSubService
{
    private readonly IServiceBusRepository serviceBusRepository;
    private IDictionary<string, IReadOnlyList<string>> _topicToSubscriptions =
        new Dictionary<string, IReadOnlyList<string>>();
    private IDictionary<string, ServiceBusReceiver> _receiverLookUp =
        new Dictionary<string, ServiceBusReceiver>();
    private IDictionary<string, ServiceBusSender> _senderLookUp =
        new Dictionary<string, ServiceBusSender>();

    public PubSubService(IServiceBusRepository serviceBusRepository)
    {
        this.serviceBusRepository = serviceBusRepository;
    }

    public async Task Setup()
    {
        _topicToSubscriptions = await RetrieveTopicAndSubscriptionNames();
        _receiverLookUp = CreateReceivers(_topicToSubscriptions);
        _senderLookUp = CreateSenders(_topicToSubscriptions);
    }

    public IDictionary<string, IReadOnlyList<string>> GetTopicsAndSubscriptionNames() =>
        _topicToSubscriptions;

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(
        string topicName,
        string subscriptionName,
        int noOfMessages
    )
    {
        var receiver = GetReceiverOrThrow(topicName, subscriptionName);
        return await receiver.PeekMessagesAsync(noOfMessages);
    }

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekDeadLetterMessages(
        string topicName,
        string subscriptionName,
        int noOfMessages
    )
    {
        var receiver = GetDeadLetterReceiverOrThrow(topicName, subscriptionName);
        return await receiver.PeekMessagesAsync(noOfMessages);
    }

    public async Task ResubmitDeadLetterMessages(
        string topicName,
        string subscriptionName,
        int fetchCount,
        TimeSpan? maxWaitTime = null,
        bool createNewMessageId = false
    )
    {
        maxWaitTime ??= new TimeSpan(0, 0, 10);

        var receiver = GetDeadLetterReceiverOrThrow(topicName, subscriptionName);
        var sender = GetSenderOrThrow(topicName);

        var deadLetterMessages = await receiver.ReceiveMessagesAsync(fetchCount, maxWaitTime);

        foreach (var message in deadLetterMessages)
        {
            var resubmittableMessage = new ServiceBusMessage(message);

            if (createNewMessageId)
            {
                var newMessageId = Guid.NewGuid();
                resubmittableMessage.MessageId = newMessageId.ToString();
            }

            await sender.SendMessageAsync(resubmittableMessage);
            await receiver.CompleteMessageAsync(message);
        }
    }

    private ServiceBusSender GetSenderOrThrow(string topicName)
    {
        if (!_senderLookUp.TryGetValue(topicName, out var sender))
        {
            throw new NotFoundException(
                $"No ServiceBusSender was found for topic named '{topicName}'."
            );
        }

        return sender;
    }

    private ServiceBusReceiver GetReceiverOrThrow(string topicName, string subscriptionName)
    {
        var topicSubscriptionPath = GetTopicSubscriptionPath(topicName, subscriptionName);
        if (!_receiverLookUp.TryGetValue(topicSubscriptionPath, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for topic/subscription path named '{topicSubscriptionPath}'."
            );
        }

        return receiver;
    }

    private ServiceBusReceiver GetDeadLetterReceiverOrThrow(
        string topicName,
        string subscriptionName
    )
    {
        var deadLetterQueueName = GetTopicSubscriptionDeadLetterPath(topicName, subscriptionName);
        if (!_receiverLookUp.TryGetValue(deadLetterQueueName, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for deadLetterQueue named '{deadLetterQueueName}'."
            );
        }

        return receiver;
    }

    private async Task<
        IDictionary<string, IReadOnlyList<string>>
    > RetrieveTopicAndSubscriptionNames()
    {
        var topicToSubscriptions = new Dictionary<string, IReadOnlyList<string>>();

        var topics = await serviceBusRepository.GetTopicsAsync();
        var topicNames = topics.Select(t => t.Path).ToList();

        foreach (var topicName in topicNames)
        {
            var subscriptions = await serviceBusRepository.GetSubscriptionsAsync(topicName);
            var subscriptionNames = subscriptions.Select(s => s.SubscriptionName).ToList();
            topicToSubscriptions[topicName] = subscriptionNames;
        }

        return topicToSubscriptions;
    }

    private IDictionary<string, ServiceBusReceiver> CreateReceivers(
        IDictionary<string, IReadOnlyList<string>> topicToSubscriptions
    )
    {
        var lookUp = new Dictionary<string, ServiceBusReceiver>();

        foreach (var topicName in topicToSubscriptions.Keys)
        {
            foreach (var subscriptionName in topicToSubscriptions[topicName])
            {
                var receiver = serviceBusRepository.CreateReceiver(topicName, subscriptionName);
                var topicSubscriptionPath = GetTopicSubscriptionPath(topicName, subscriptionName);
                lookUp.Add(topicSubscriptionPath, receiver);

                var topicSubscriptionDeadLetterPath = GetTopicSubscriptionDeadLetterPath(
                    topicName,
                    subscriptionName
                );
                var deadLetterReceiver = serviceBusRepository.CreateReceiver(
                    topicSubscriptionDeadLetterPath
                );
                lookUp.Add(topicSubscriptionDeadLetterPath, deadLetterReceiver);
            }
        }

        return lookUp;
    }

    private IDictionary<string, ServiceBusSender> CreateSenders(
        IDictionary<string, IReadOnlyList<string>> topicToSubscriptions
    )
    {
        var lookUp = new Dictionary<string, ServiceBusSender>();

        foreach (var topicName in topicToSubscriptions.Keys)
        {
            var sender = serviceBusRepository.CreateSender(topicName);
            lookUp.Add(topicName, sender);
        }

        return lookUp;
    }

    private static string GetTopicSubscriptionPath(string topicName, string subscriptionName)
    {
        return $"{topicName}/Subscriptions/{subscriptionName}";
    }

    private static string GetTopicSubscriptionDeadLetterPath(
        string topicName,
        string subscriptionName
    )
    {
        return $"{topicName}/Subscriptions/{subscriptionName}/$deadletterqueue";
    }
}
