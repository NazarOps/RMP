using System.Runtime.Serialization.Json;
using System.Text.Json;

namespace RMP;

public class StateService
{
    public StateService()
    {
        // here we will load the settings and state from disk
    }

    // placeholder for settings
    private string _settings = "settings";
    // placeholder for state
    private string _state = "state";
    public static void Jsonize<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, typeof(T));

    }
    public static void Dejsonize<T>(string json)
    {
        var obj = JsonSerializer.Deserialize<T>(json);

        // here we load the actual state
    }
}
