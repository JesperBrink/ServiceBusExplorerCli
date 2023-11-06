using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class ResubmitDeadLetterMessagesInSubscriptionCommand : ICommand
{
    private readonly IPubSubService _pubSubService;

    public string Title { get; }

    public ResubmitDeadLetterMessagesInSubscriptionCommand(IPubSubService pubSubService)
    {
        _pubSubService = pubSubService;
        Title = "Resubmit deadletter messages in a subscription";
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

        if (subscriptions.Count > 1)
        {
            Console.WriteLine(
                "Resubmitting deadletter messages for topic with multiple subscriptions will resubmit the message to ALL subscriptsions."
            );
            var userWantsToContinue = PromtUtil.GetBoolInput("Are you sure you want to continue? ");

            if (!userWantsToContinue)
            {
                return;
            }
        }

        Console.Clear();

        var noOfMessagesToResubmit = PromtUtil.GetIntInput(
            "Number of messages to resubmit: ",
            minAllowedInput: 1,
            maxAllowedInput: 100
        );
        await _pubSubService.ResubmitDeadLetterMessages(
            topicName,
            chosenSubscription,
            noOfMessagesToResubmit
        );

        Console.Clear();

        Console.WriteLine("Deadletter messages have been resubmitted");
    }
}
