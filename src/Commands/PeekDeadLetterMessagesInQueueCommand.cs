using Azure.Messaging.ServiceBus;
using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class PeekDeadLetterMessagesInQueueCommand : ICommand
{
    private readonly IQueueService _queueService;

    public string Title { get; }

    public PeekDeadLetterMessagesInQueueCommand(IQueueService queueService)
    {
        _queueService = queueService;
        Title = "Peek deadletter messages in a queue";
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

        var noOfMessagesToPeek = PromtUtil.GetIntInput(
            "Number of deadletter messages to peek: ",
            minAllowedInput: 1,
            maxAllowedInput: 100
        );
        var messages = await _queueService.PeekDeadLetterMessages(chosenQueue, noOfMessagesToPeek);

        Console.Clear();

        Console.WriteLine("Peeked deadletter messages in queue:");
        var messageDescriptions = GetMessageDescriptions(messages);
        PromtUtil.WriteList(messageDescriptions);
    }

    private IList<string> GetMessageDescriptions(IReadOnlyList<ServiceBusReceivedMessage> messages)
    {
        return messages
            .Select(
                m =>
                    $"- MessageId: {m.MessageId}, EnqueuedTimeUtc: {m.EnqueuedTime}, Body: {m.Body}"
            )
            .ToList();
    }
}
