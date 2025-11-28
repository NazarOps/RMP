using System.Text.Json;

namespace RMP;

public static class StateService
{

    /// Saves any object as JSON to a file.
    /// 
    public static void Jsonize<T>(T value, string filePath)
    {
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
    }

    /// Loads an object from JSON file. Returns default(T) if file is missing or invalid.

    public static T Dejsonize<T>(string filePath) where T : new()
    {
        if (!File.Exists(filePath))
            return new T();

        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        catch
        {
            // In case of invalid JSON
            return new T();
        }
    }
}
