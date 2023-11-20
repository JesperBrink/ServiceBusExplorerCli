using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using ServiceBusExplorerCli.Commands;
using ServiceBusExplorerCli.Commands.Interface;
using ServiceBusExplorerCli.Models;
using ServiceBusExplorerCli.Repositories;
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
        var serviceBusRepository = new ServiceBusRepository(
            new ServiceBusClient(connectionConfig.ConnectionString),
            new ManagementClient(connectionConfig.ConnectionString)
        );

        var queueService = new QueueService(serviceBusRepository);
        await queueService.Setup();

        var pubSubService = new PubSubService(serviceBusRepository);
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
