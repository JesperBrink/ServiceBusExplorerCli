using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Commands;

public class ListDeadLetterQueuesWithMessagesCommand : ICommand
{
    private readonly IQueueService _queueService;
    private readonly IPubSubService _pubService;

    public string Title { get; }

    public ListDeadLetterQueuesWithMessagesCommand(
        IQueueService queueService,
        IPubSubService pubService
    )
    {
        _queueService = queueService;
        _pubService = pubService;
        Title = "List all DeadLetterQueues with messages";
    }

    public async Task Execute()
    {
        var queuesWithDeadLetterMessages = await GetQueuesWithDeadLetterMessages();
        if (queuesWithDeadLetterMessages.Any())
        {
            Console.WriteLine("Following queues have DeadLetterMessages:");
            PromtUtil.WriteList(queuesWithDeadLetterMessages);
        }
        else
        {
            Console.WriteLine("No queues have any DeadLetterMessages.");
        }

        var subscriptionsWithDeadLetterMessages = await GetSubscriptionsWithDeadLetterMessages();
        if (subscriptionsWithDeadLetterMessages.Any())
        {
            Console.WriteLine("Following topic/subscriptions have DeadLetterMessages:");
            PromtUtil.WriteList(subscriptionsWithDeadLetterMessages);
        }
        else
        {
            Console.WriteLine("No topic/subscriptions have any DeadLetterMessages.");
        }
    }

    private async Task<IList<string>> GetQueuesWithDeadLetterMessages()
    {
        var queuesWithDeadLetterMessages = new List<string>();

        var queues = _queueService.GetQueueNames();
        foreach (var queue in queues)
        {
            var deadLetterMessages = await _queueService.PeekDeadLetterMessages(queue, 1);
            if (deadLetterMessages.Any())
            {
                queuesWithDeadLetterMessages.Add(queue);
            }
        }

        return queuesWithDeadLetterMessages;
    }

    private async Task<IList<string>> GetSubscriptionsWithDeadLetterMessages()
    {
        var subscriptionsWithDeadLetterMessages = new List<string>();

        var topicsAndSubscriptionNames = _pubService.GetTopicsAndSubscriptionNames();
        foreach (var entry in topicsAndSubscriptionNames)
        {
            foreach (var subscription in entry.Value)
            {
                var deadLetterMessages = await _pubService.PeekDeadLetterMessages(
                    entry.Key,
                    subscription,
                    1
                );
                if (deadLetterMessages.Any())
                {
                    subscriptionsWithDeadLetterMessages.Add($"{entry.Key}/{subscription}");
                }
            }
        }

        return subscriptionsWithDeadLetterMessages;
    }
}
