using ServiceBusExplorerCli.Services.Interface;

namespace ServiceBusExplorerCli.Services;

public class CliService : ICliService
{
    public void Start()
    {
        Console.WriteLine("Starting ServiceBusExplorer CLI!");

        var connectionConfig = ConnectionConfigUtil.GetConnectionConfig();

        var queueService = new QueueService(connectionConfig.ConnectionString);
        var pubSubService = new PubSubService(connectionConfig.ConnectionString);
    }
}
