using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class ListTopicsCommand : ICommand
{
    private readonly IPubSubService _pubService;

    public string Title { get; }

    public ListTopicsCommand(IPubSubService pubService)
    {
        _pubService = pubService;
        Title = "List all topics";
    }

    public Task Execute()
    {
        Console.WriteLine("List of all topics:");

        var topicsAndSubscriptionNames = _pubService.GetTopicsAndSubscriptionNames();
        PromtUtil.WriteList(topicsAndSubscriptionNames.Keys.ToList());

        return Task.CompletedTask;
    }
}
