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

    public static int GetIntInput(string? messageToUser)
    {
        Console.Write(messageToUser);

        do
        {
            var rawInputFromUser = Console.ReadLine();
            if (int.TryParse(rawInputFromUser, out int intInputFromUser))
            {
                return intInputFromUser;
            }

            Console.WriteLine("Sorry, but the input must be an integer.");
            Console.Write("Try again: ");
        } while (true);
    }
}
