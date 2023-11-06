using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class ResubmitDeadLetterMessagesInQueueCommand : ICommand
{
    private readonly IQueueService _queueService;

    public string Title { get; }

    public ResubmitDeadLetterMessagesInQueueCommand(IQueueService queueService)
    {
        _queueService = queueService;
        Title = "Resubmit deadletter messages in queue";
    }

    public async Task Execute()
    {
        Console.WriteLine("List of all queues:");
        var queueNames = _queueService.GetQueueNames();
        PromtUtil.WriteIndexedList(queueNames.ToList());
        var chosenIndex = PromtUtil.GetIntInput(
            "Index of queue to choose: ",
            maxAllowedInput: queueNames.Count - 1
        );
        var chosenQueue = queueNames[chosenIndex];

        Console.Clear();

        var noOfMessagesToResubmit = PromtUtil.GetIntInput(
            "Number of messages to resubmit: ",
            minAllowedInput: 1,
            maxAllowedInput: 100
        );
        await _queueService.ResubmitDeadLetterMessages(chosenQueue, noOfMessagesToResubmit);

        Console.Clear();

        Console.WriteLine("Deadletter messages have been resubmitted");
    }
}
