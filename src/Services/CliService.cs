using ServiceBusExplorerCli.Commands;
using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Models;
using ServiceBusExplorerCli.Services.Interface;
using ServiceBusExplorerCli.Util;

namespace ServiceBusExplorerCli.Services;

public class CliService : ICliService
{
    public async Task Start()
    {
        var connectionConfig = ConnectionConfigUtil.GetConnectionConfig();
        var commands = await IntializeCommands(connectionConfig);
        var commandTitles = commands.Select(x => x.Title).ToList();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("The following commands are possible:");

            PromtUtil.WriteIndexedList(commandTitles);
            var chosenIndex = PromtUtil.GetIntInput(
                "Index of operation to perform: ",
                maxAllowedInput: commands.Count - 1
            );

            Console.Clear();
            await commands[chosenIndex].Execute();

            PromtUtil.WaitForUserToPressEnter();
        }
    }

    private async Task<IReadOnlyList<ICommand>> IntializeCommands(ConnectionConfig connectionConfig)
    {
        var queueService = new QueueService(connectionConfig.ConnectionString);
        await queueService.Setup();

        var pubSubService = new PubSubService(connectionConfig.ConnectionString);
        await pubSubService.Setup();

        return new List<ICommand>
        {
            new ListQueuesCommand(queueService),
            new ListTopicsCommand(pubSubService),
            new ListSubscriptionsInTopicsCommand(pubSubService),
            new ListDeadLetterQueuesWithMessagesCommand(queueService, pubSubService),
            new PeekMessagesInQueueCommand(queueService),
            new PeekMessagesInSubscriptionCommand(pubSubService),
            new PeekDeadLetterMessagesInQueueCommand(queueService),
            new PeekDeadLetterMessagesInSubscriptionCommand(pubSubService),
            new ResubmitDeadLetterMessagesInQueueCommand(queueService),
            new ResubmitDeadLetterMessagesInSubscriptionCommand(pubSubService)
        };
    }
}
