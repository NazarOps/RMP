namespace RMP.Interfaces;
public interface ILogService
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
}
