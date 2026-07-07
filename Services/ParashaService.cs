using BeitKnesetDisplay.Models;
using System.IO;
using System.Text.Json;

public class ParashaService
{
    private readonly Dictionary<string, ParashaInfo> _data;

    public ParashaService()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "ParashaContent.json");

        string json = File.ReadAllText(path);

        _data =
            JsonSerializer.Deserialize<
                Dictionary<string, ParashaInfo>>(json)
            ?? new();
    }

    public ParashaInfo? GetParasha(string name)
    {
        _data.TryGetValue(name, out var info);
        return info;
    }
}