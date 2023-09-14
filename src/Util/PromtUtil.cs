namespace ServiceBusExplorerCli.Util;

public static class PromtUtil
{
    public static void WriteIndexedList(IList<string> listToWrite)
    {
        for (var i = 0; i < listToWrite.Count; i++)
        {
            Console.WriteLine($"{i}: {listToWrite[i]}");
        }
    }

    public static void WriteList(IList<string> listToWrite)
    {
        foreach (var element in listToWrite)
        {
            Console.WriteLine(element);
        }
    }

    public static string? GetInput(string? messageToUser)
    {
        Console.Write(messageToUser);
        return Console.ReadLine();
    }
}
