using RMP.Interfaces;

namespace RMP.Services;

internal class LogService : ILogService
{
    string logPath = "logs.txt";
    public void LogError(string message)
    {
        try
        {
            File.AppendAllText(logPath, $"ERROR: {message}{Environment.NewLine}");
        }
        catch (Exception ex) { Console.Write($"Logger failed:{ex.Message}"); }
    }

    public void LogInfo(string message)
    {
        try
        {
            File.AppendAllText(logPath, $"INFO: {message}{Environment.NewLine}");
        }
        catch (Exception ex) { Console.Write($"Logger failed:{ex.Message}"); }
    }

    public void LogWarning(string message)
    {
        try
        {
            File.AppendAllText(logPath, $"WARNING: {message}{Environment.NewLine}");
        }
        catch (Exception ex) { Console.Write($"Logger failed:{ex.Message}"); }
    }
}
