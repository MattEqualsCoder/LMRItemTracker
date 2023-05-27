using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LMRItemTracker.Configs;

public class ConfigService
{
    public TrackerConfig Config { get; set; }

    private ILogger<ConfigService> _logger;
    
    private static readonly IDeserializer s_deserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
        .Build();

    public ConfigService(ILogger<ConfigService> logger)
    {
        _logger = logger;
        Config = new();
        LoadConfig();
    }

    public void LoadConfig()
    {
        var configText = LoadBuiltInYamlFile();
        Config = s_deserializer.Deserialize<TrackerConfig>(configText);
    }
    
    private static string LoadBuiltInYamlFile()
    {
        var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("LMRItemTracker.Configs.config.yml");
        if (stream == null)
            return "";
        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }
}