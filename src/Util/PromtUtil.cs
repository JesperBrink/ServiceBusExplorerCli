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

    public static string GetStringInput(string? messageToUser)
    {
        Console.Write(messageToUser);
        do
        {
            var rawInputFromUser = Console.ReadLine();
            if (string.IsNullOrEmpty(rawInputFromUser))
            {
                Console.WriteLine("Sorry, but the input cannot be empty.");
                Console.Write("Try again: ");
                continue;
            }

            return rawInputFromUser;
        } while (true);
    }

    public static int GetIntInput(
        string? messageToUser,
        int minAllowedInput = 0,
        int maxAllowedInput = int.MaxValue
    )
    {
        Console.Write(messageToUser);

        do
        {
            var rawInputFromUser = Console.ReadLine();
            if (!int.TryParse(rawInputFromUser, out int intInputFromUser))
            {
                Console.WriteLine("Sorry, but the input must be an integer.");
                Console.Write("Try again: ");
                continue;
            }

            if (intInputFromUser < minAllowedInput || intInputFromUser > maxAllowedInput)
            {
                Console.WriteLine(
                    $"Sorry, but the input must be in the range {minAllowedInput} to {maxAllowedInput}."
                );
                Console.Write("Try again: ");
                continue;
            }

            return intInputFromUser;
        } while (true);
    }
}
