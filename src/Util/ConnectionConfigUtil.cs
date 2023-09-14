using System.Text.Json;
using ServiceBusExplorerCli.Models;

namespace ServiceBusExplorerCli.Util;

public static class ConnectionConfigUtil
{
    private const string ConnectionConfigPath = ".serviceBusExplorerCliConfig";

    public static ConnectionConfig GetConnectionConfig()
    {
        var configs = LoadConnectionConfigs();

        if (!configs.Any())
        {
            Console.WriteLine("No connections configurations was found.");
            var newConfig = CreateConnectionConfig();
            configs.Add(newConfig);
            SaveConnectionConfig(newConfig);
        }

        Console.WriteLine("Choose between the following configs:");
        PromtUtil.WriteIndexedList(configs.Select(x => x.Name).ToList());
        var chosenIndex = int.Parse(PromtUtil.GetInput("Enter id of config: "));

        return configs[chosenIndex];
    }

    private static ConnectionConfig CreateConnectionConfig()
    {
        Console.WriteLine("Creating a new connection config.");

        Console.Write("Name: ");
        var name = Console.ReadLine();

        Console.Write("ConnectionString: ");
        var connectionString = Console.ReadLine();

        return new ConnectionConfig(name, connectionString);
    }

    private static void SaveConnectionConfig(ConnectionConfig config)
    {
        var existingConnectionConfigs = LoadConnectionConfigs();
        existingConnectionConfigs.Add(config);
        File.WriteAllText(
            ConnectionConfigPath,
            JsonSerializer.Serialize(existingConnectionConfigs)
        );
    }

    private static IList<ConnectionConfig> LoadConnectionConfigs()
    {
        if (File.Exists(ConnectionConfigPath))
        {
            using StreamReader reader = new(ConnectionConfigPath);
            return JsonSerializer.Deserialize<IList<ConnectionConfig>>(reader.ReadToEnd())
                ?? new List<ConnectionConfig>();
        }

        return new List<ConnectionConfig>();
    }
}
