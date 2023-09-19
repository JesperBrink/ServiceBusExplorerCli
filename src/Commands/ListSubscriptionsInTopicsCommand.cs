using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class ListSubscriptionsInTopicsCommand : ICommand
{
    private readonly IPubSubService _pubService;

    public string Title { get; }

    public ListSubscriptionsInTopicsCommand(IPubSubService pubService)
    {
        _pubService = pubService;
        Title = "List all subscriptions in a topic";
    }

    public Task Execute()
    {
        Console.WriteLine("List of all topics:");

        var topicsAndSubscriptionNames = _pubService.GetTopicsAndSubscriptionNames();
        var topics = topicsAndSubscriptionNames.Keys.ToList();
        PromtUtil.WriteIndexedList(topics);

        var chosenIndex = PromtUtil.GetIntInput(
            "Index of topic to choose: ",
            maxAllowedInput: topics.Count - 1
        );
        var topicName = topics[chosenIndex];

        Console.Clear();

        Console.WriteLine($"List of subscriptions in {topicName}:");
        PromtUtil.WriteList(topicsAndSubscriptionNames[topicName].ToList());

        return Task.CompletedTask;
    }
}
