using Azure.Messaging.ServiceBus;
using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class PeekDeadLetterMessagesInSubscriptionCommand : ICommand
{
    private readonly IPubSubService _pubSubService;

    public string Title { get; }

    public PeekDeadLetterMessagesInSubscriptionCommand(IPubSubService pubSubService)
    {
        _pubSubService = pubSubService;
        Title = "Peek deadletter messages in a subscription";
    }

    public async Task Execute()
    {
        Console.WriteLine("List of all topics:");

        var topicsAndSubscriptionNames = _pubSubService.GetTopicsAndSubscriptionNames();
        var topics = topicsAndSubscriptionNames.Keys.ToList();
        PromtUtil.WriteIndexedList(topics);

        var chosenIndex = PromtUtil.GetIntInput(
            "Index of topic to choose: ",
            maxAllowedInput: topics.Count - 1
        );
        var topicName = topics[chosenIndex];

        Console.Clear();

        Console.WriteLine($"List of subscriptions in {topicName}:");
        var subscriptions = topicsAndSubscriptionNames[topicName].ToList();
        PromtUtil.WriteIndexedList(subscriptions);

        chosenIndex = PromtUtil.GetIntInput(
            "Index of subscription to choose: ",
            maxAllowedInput: subscriptions.Count - 1
        );
        var chosenSubscription = subscriptions[chosenIndex];

        Console.Clear();

        var noOfMessagesToPeek = PromtUtil.GetIntInput(
            "Number of deadletter messages to peek: ",
            minAllowedInput: 1,
            maxAllowedInput: 100
        );
        var messages = await _pubSubService.PeekDeadLetterMessages(
            topicName,
            chosenSubscription,
            noOfMessagesToPeek
        );

        Console.Clear();

        Console.WriteLine("Peeked deadletter messages in subscription:");
        var messageDescriptions = GetMessageDescriptions(messages);
        PromtUtil.WriteList(messageDescriptions);
    }

    private static IList<string> GetMessageDescriptions(
        IReadOnlyList<ServiceBusReceivedMessage> messages
    )
    {
        return messages
            .Select(
                m =>
                    $"- MessageId: {m.MessageId}, EnqueuedTimeUtc: {m.EnqueuedTime}, Body: {m.Body}"
            )
            .ToList();
    }
}
