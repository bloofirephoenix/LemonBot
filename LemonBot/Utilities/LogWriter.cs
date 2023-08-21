using System.Text;

namespace LemonBot.Utilities;

public class LogWriter : TextWriter
{
    private readonly TextWriter _out;
    private readonly Logger.LogLevel _defaultLevel;
    
    public LogWriter(TextWriter output, Logger.LogLevel defaultLevel)
    {
        _out = output;
        _defaultLevel = defaultLevel;
    }
    
    public override Encoding Encoding => Encoding.UTF8;
    
    public override void Write(string? value)
    {
        _out.Write(value);
    }

    public override void WriteLine(string? value)
    {
        Log(_defaultLevel, value);
    }

    public void Log(Logger.LogLevel level, string? value)
    {
        if (level == Logger.LogLevel.Debug && !Logger.DebugMessages)
            return;
        
        Console.ForegroundColor = ConsoleColor.Gray;
        _out.Write("[");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        _out.Write(DateTime.Now.ToString(TimeFormat.Format));
        
        Console.ForegroundColor = ConsoleColor.Gray;
        _out.Write("] ");
        
        Console.ForegroundColor = ConsoleColor.Gray;
        _out.Write("[");

        Console.ForegroundColor = Logger.GetLevelColor(level);
        _out.Write(level);
        
        Console.ForegroundColor = ConsoleColor.Gray;
        _out.Write("]: ");
        
        Console.ResetColor();
        _out.WriteLine(value);
    }
}