using ServiceBusExplorerCli.Services;

namespace ServiceBusExplorerCli;

public class Program
{
    static async Task Main(string[] args)
    {
        var cliService = new CliService();
        await cliService.Start();
    }
}
