using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class ListQueuesCommand : ICommand
{
    private readonly IQueueService _queueService;

    public string Title { get; }

    public ListQueuesCommand(IQueueService queueService)
    {
        _queueService = queueService;
        Title = "List all queues";
    }

    public void Execute()
    {
        var queueNames = _queueService.GetQueueNames();
        PromtUtil.WriteList(queueNames.ToList());
    }
}
