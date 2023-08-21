namespace LemonBot;

public static class TerminalManager
{
    public static void Start()
    {
        while (true)
        {
            string message = Console.ReadLine()!.ToLower();
            string[] args = message.Split(' ');

            switch (args[0])
            {
                case "stop":
                    return;
            }
        }
    }
}