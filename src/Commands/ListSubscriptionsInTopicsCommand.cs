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

    public void Execute()
    {
        var topicsAndSubscriptionNames = _pubService.GetTopicsAndSubscriptionNames();
        var topics = topicsAndSubscriptionNames.Keys.ToList();
        PromtUtil.WriteIndexedList(topics);
        
        var chosenIndex = int.Parse(PromtUtil.GetInput("Index of topic to choose: "));
        var topicName = topics[chosenIndex];
        PromtUtil.WriteList(topicsAndSubscriptionNames[topicName].ToList());
    }
}
