using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LMRItemTracker.Configs;

public class ConfigService
{
    public TrackerConfig Config { get; set; } = new(null, null, null, null, null, null, null, null);
    
    private static readonly IDeserializer s_deserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
        .Build();
    
    private readonly ILogger<ConfigService> _logger;
    
    public ConfigService(ILogger<ConfigService> logger)
    {
        _logger = logger;
        LoadConfig();
    }
    
    public bool LoadedSuccessfully { get; private set; }

    public void LoadConfig()
    {
        try
        {
            var configFile = LoadBuiltInConfigs();
            Config.Merge(configFile);
            if (GetUserYamlFile(out configFile) && configFile != null)
            {
                Config.Merge(configFile);
            }

            LoadedSuccessfully = true;
        }
        catch (YamlException e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
    }
    
    private TrackerConfig LoadBuiltInConfigs()
    {
        var items = LoadBuiltInYamlFile<ItemsConfig>("items.yml");
        var locations = LoadBuiltInYamlFile<LocationsConfig>("locations.yml");
        var regions = LoadBuiltInYamlFile<RegionsConfig>("regions.yml");
        var responses = LoadBuiltInYamlFile<ResponseConfig>("responses.yml");
        var custom = LoadBuiltInYamlFile<CustomConfig>("custom.yml");
        var twitch = LoadBuiltInYamlFile<TwitchConfig>("twitch.yml");
        var npcs = LoadBuiltInYamlFile<NpcsConfig>("npcs.yml");
        var bosses = LoadBuiltInYamlFile<BossesConfig>("bosses.yml");
        return new TrackerConfig(items, locations, regions, responses, custom, twitch, npcs, bosses);
    }
    
    private T? LoadBuiltInYamlFile<T>(string filename)
    {
        try
        {
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"LMRItemTracker.Configs.yaml.{filename}");
            if (stream == null)
                return default;
            using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
            var yaml = reader.ReadToEnd();
            var config = s_deserializer.Deserialize<T>(yaml);
            _logger.LogInformation("Loaded built in yaml file {Filename}", filename);
            return config;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not load built in config file {Filename}", filename);
            throw new YamlException($"Could not load built in config filefile ${filename}", e);
        }
        
    }

    private bool GetUserYamlFile(out TrackerConfig? config)
    {
        if (LoadUserConfigs(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "LMRItemTracker", out config))
        {
            return config != null;
        }
        else if (LoadUserConfigs(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), out config))
        {
            return config != null;
        }

        return false;
    }

    public bool LoadUserConfigs(string? path, out TrackerConfig? config)
    {
        var items = LoadUserYamlFile<ItemsConfig>(path + Path.DirectorySeparatorChar + "items.yml");
        var locations = LoadUserYamlFile<LocationsConfig>(path + Path.DirectorySeparatorChar + "locations.yml");
        var regions = LoadUserYamlFile<RegionsConfig>(path + Path.DirectorySeparatorChar + "regions.yml");
        var responses = LoadUserYamlFile<ResponseConfig>(path + Path.DirectorySeparatorChar + "responses.yml");
        var custom = LoadUserYamlFile<CustomConfig>(path + Path.DirectorySeparatorChar + "custom.yml");
        var twitch = LoadUserYamlFile<TwitchConfig>(path + Path.DirectorySeparatorChar + "twitch.yml");
        var npcs = LoadUserYamlFile<NpcsConfig>(path + Path.DirectorySeparatorChar + "npcs.yml");
        var bosses = LoadUserYamlFile<BossesConfig>(path + Path.DirectorySeparatorChar + "bosses.yml");
        if (items == null && locations == null && regions == null && responses == null && custom == null && twitch == null && npcs == null && bosses == null)
        {
            config = null;
            return false;
        }
        config = new TrackerConfig(items, locations, regions, responses, custom, twitch, npcs, bosses);
        return true;
    }
    
    private T? LoadUserYamlFile<T>(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                _logger.LogInformation("Could not locate config file {Path}", path);
                return default;
            }

            string yaml;
            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
            {
                yaml = streamReader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(yaml))
            {
                _logger.LogWarning("Yaml file {Path} is empty", path);
                return default;
            }

            var config = s_deserializer.Deserialize<T>(yaml);
            _logger.LogInformation("Loading user yaml file {Path}", path);
            return config;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not load user config file {Filename}", path);
            throw new YamlException($"Could not load config ${path}", e);
        }
    }

}