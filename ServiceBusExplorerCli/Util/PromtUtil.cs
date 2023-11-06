namespace ServiceBusExplorerCli.Util;

public static class PromtUtil
{
    public static void WriteIndexedList(IList<string> listToWrite, int indentationLevel = 2)
    {
        var indentation = new String(' ', indentationLevel);

        for (var i = 0; i < listToWrite.Count; i++)
        {
            Console.WriteLine($"{indentation}{i}: {listToWrite[i]}");
        }
    }

    public static void WriteList(IList<string> listToWrite, int indentationLevel = 2)
    {
        var indentation = new String(' ', indentationLevel);

        foreach (var element in listToWrite)
        {
            Console.WriteLine($"{indentation}{element}");
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

    public static bool GetBoolInput(string? messageToUser)
    {
        Console.Write($"(Y/N) {messageToUser}");
        do
        {
            var rawInputFromUser = Console.ReadLine();
            if (string.IsNullOrEmpty(rawInputFromUser))
            {
                Console.WriteLine("Sorry, but the input cannot be empty.");
                Console.Write("Try again: ");
                continue;
            }

            if (rawInputFromUser.ToLower().Equals("y") || rawInputFromUser.ToLower().Equals("yes"))
            {
                return true;
            }

            if (rawInputFromUser.ToLower().Equals("n") || rawInputFromUser.ToLower().Equals("no"))
            {
                return false;
            }

            Console.WriteLine("Sorry, but you must enter either 'yes' (y) or 'no' (n).");
            Console.Write("Try again: ");
        } while (true);
    }

    public static void WaitForUserToPressEnter()
    {
        Console.WriteLine("Press enter to return to main menu.");
        Console.ReadLine();
    }
}
