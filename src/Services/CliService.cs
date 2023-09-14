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

        Console.WriteLine($"Config {connectionConfig.Name} was successfully loaded.\n");

        while (true)
        {
            Console.WriteLine("The following commands are possible:");
            PromtUtil.WriteIndexedList(commandTitles);
            var chosenIndex = int.Parse(PromtUtil.GetInput("Index of operation to perform: "));
            commands[chosenIndex].Execute();
            Console.WriteLine();
        }
    }

    private async Task<IReadOnlyList<ICommand>> IntializeCommands(ConnectionConfig connectionConfig)
    {
        var queueService = new QueueService(connectionConfig.ConnectionString);
        await queueService.Setup();

        var pubSubService = new PubSubService(connectionConfig.ConnectionString);
        await pubSubService.Setup();

        return new List<ICommand> { new ListQueuesCommand(queueService) };
    }
}
