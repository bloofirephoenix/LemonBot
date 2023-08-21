using Discord;

namespace LemonBot.Utilities;

public class Logger
{
    public static Logger Instance = null!;
    private static LogWriter _writer = null!;
#if DEBUG
    public static bool DebugMessages = true;
#else
    public static bool DebugMessages = false;
#endif
    
    public Logger()
    {
        if (Instance != null)
            throw new Exception("Only one logger can be initialized");
        
        Instance = this;
        _writer = new(Console.Out, LogLevel.Info);
        Console.SetOut(_writer);
        
        
    }
    
    public enum LogLevel
    {
        Info,
        Error,
        Debug,
        Warning
    }

    public static void Log(LogLevel level, string message)
    {
        _writer.Log(level, message);
    }

    public static ConsoleColor GetLevelColor(LogLevel level) => level switch
    {
        LogLevel.Info => ConsoleColor.Green,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Debug => ConsoleColor.Blue,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unknown Debug Level")
    };

    public static void Info(string message) => Log(LogLevel.Info, message);
    public static void Error(string message) => Log(LogLevel.Error, message);
    public static void Warning(string message) => Log(LogLevel.Warning, message);
    public static void Debug(string message) => Log(LogLevel.Debug, message);
    
    public static Task DiscordLog(LogMessage msg)
    {
        LogLevel level = msg.Severity switch
        {
            LogSeverity.Critical => LogLevel.Error,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Info
        };
        
        Log(level, msg.Message);
        
        return Task.CompletedTask;
    }
}