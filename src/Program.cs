using ServiceBusExplorerCli.Services;

namespace ServiceBusExplorerCli;

public class Program
{
    static void Main(string[] args)
    {
        var cliService = new CliService();
        cliService.Start();
    }
}
