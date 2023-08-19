using System;
using LMRItemTracker.Configs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LMRItemTracker.VoiceTracker;

public class HintService
{
    private readonly ILogger<HintService> _logger;
    private readonly TrackerConfig _trackerConfig;
    private readonly TextToSpeechService _ttsService;
    private readonly Dictionary<string, string> _itemLocations = new();
    private readonly Dictionary<string, List<string>> _locationItems = new();
    private readonly Dictionary<string, string> _npcLocations = new();
    private readonly Dictionary<string, string> _locationNpcs = new();
    private readonly Dictionary<string, int> _hintsGiven = new();
    private readonly Dictionary<string, List<string>> _sealLocations = new();

    private readonly List<string> _seals = new List<string>()
        { "Origin Seal", "Birth Seal", "Life Seal", "Death Seal" };
    private string? _randomizerPath;
    private bool _hintsEnabled;
    private bool _spoilersEnabled;
    
    public HintService(ConfigService configService, VoiceRecognitionService voiceService, TextToSpeechService ttsService, ILogger<HintService> logger, ChoiceService choices)
    {
        _logger = logger;
        _trackerConfig = configService.Config;
        _ttsService = ttsService;
        _randomizerPath = Properties.Settings.Default.RandomizerPath;
        
        if (!configService.LoadedSuccessfully)
        {
            return;
        }

        voiceService.AddCommand("enable hints", new GrammarBuilder().Append("Hey tracker, enable hints"), result => { EnableHints(); });
        voiceService.AddCommand("enable spoilers", new GrammarBuilder().Append("Hey tracker, enable spoilers"), result => { EnableSpoilers(); });
        voiceService.AddCommand("disable", new GrammarBuilder().Append("Hey tracker, disable ").OneOf("hints", "spoilers"), result => { Disable(); });

        voiceService.AddCommand("item hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("where is the ", "where can I find the ")
                .Append(choices.ItemKey, choices.GetItemNames()),
            result =>
            {
                var item = choices.GetItemFromResult(result, out string itemName);
                if (_hintsEnabled || _spoilersEnabled)
                {
                    GiveItemHint(item);    
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.NeedToEnableHints);
                }
            }
        );
        
        voiceService.AddCommand("location hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("what's at ", "what can I get at ")
                .Append(choices.LocationKey, choices.GetLocationNames()),
            result =>
            {
                var location = choices.GetLocationFromResult(result, out string locationName);
                if (_hintsEnabled || _spoilersEnabled)
                {
                    GiveLocationHint(location);
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.NeedToEnableHints);
                }
            }
        );
        
        voiceService.AddCommand("npc hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("where is ", "where can I find ")
                .Append(choices.NpcKey, choices.GetNpcNames()),
            result =>
            {
                var npc = choices.GetNpcConfigFromResult(result);
                if (_hintsEnabled || _spoilersEnabled)
                {
                    GiveNpcHint(npc);    
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.NeedToEnableHints);
                }
            }
        );
        
        voiceService.AddCommand("npc location hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("who's at ", "who is at ")
                .Append(choices.LocationKey, choices.GetNpcLocationNames()),
            result =>
            {
                var location = choices.GetNpcLocationConfigFromResult(result);
                if (_hintsEnabled || _spoilersEnabled)
                {
                    GiveNpcLocationHint(location);
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.NeedToEnableHints);
                }
            }
        );
        
        voiceService.AddCommand("region hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("what's in ", "what all is in ")
                .Append(choices.RegionKey, choices.GetRegionNames()),
            result =>
            {
                var regionConfig = choices.GetRegionFromResult(result, out string locationName);
                if (_hintsEnabled || _spoilersEnabled)
                {
                    GiveRegionHint(regionConfig);
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.NeedToEnableHints);
                }
            }
        );
        
        voiceService.AddCommand("ankh hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("where can I find the ankh jewels", "where are the ankh jewels"),
            result =>
            {
                if (_hintsEnabled || _spoilersEnabled)
                {
                    GiveAnkhHints();
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.NeedToEnableHints);
                }
            }
        );
        
        voiceService.AddCommand("seal hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .Append("what is the ")
                .Append(choices.SealKey, choices.GetSealNames())
                .Append("used for?"),
            result =>
            {
                var seal = choices.GetSealFromResult(result);
                if (_spoilersEnabled)
                {
                    GiveSealSpoiler(seal);
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.NeedToEnableHints);
                }
            }
        );
    }

    public TrackerService? TrackerService { get; set; }

    public bool UpdateRandomizerPath(string randomizerPath)
    {
        if (string.IsNullOrWhiteSpace(randomizerPath))
        {
            _hintsEnabled = false;
            _spoilersEnabled = false;
            _randomizerPath = null;
            return false;
        }

        var files = Directory.GetFiles(randomizerPath);

        if (files.All(x => Path.GetExtension(x) != ".jar"))
        {
            _ttsService.Say(_trackerConfig.Responses.InvalidRandomizerPath);
            return false;
        }

        _logger.LogInformation("Randomizer path: {Path}", _randomizerPath);
        _randomizerPath = randomizerPath;
        if (_hintsEnabled || _spoilersEnabled)
        {
            LoadSpoilerLog();
        }
        return true;
    }

    public bool LoadSpoilerLog()
    {
        if (string.IsNullOrWhiteSpace(_randomizerPath))
        {
            _logger.LogInformation("No randomizer path in config file");
            _ttsService.Say(_trackerConfig.Responses.NoRandomizerPath);
            return false;
        }
        var directory = new DirectoryInfo(_randomizerPath!);
        if (!directory.Exists || directory.GetDirectories().Length == 0)
        {
            _logger.LogInformation("Randomizer directory not found");
            _ttsService.Say(_trackerConfig.Responses.NoSeedFolderFound);
            return false;
        }
        var latestFolder = directory.GetDirectories().OrderByDescending(x => x.CreationTime).First();
        if (latestFolder == null) 
        {
            _logger.LogInformation("Not able to find folder within randomizer directory");
            _ttsService.Say(_trackerConfig.Responses.NoSeedFolderFound);
            return false;
        }
        var spoilerFile = latestFolder.FullName + Path.DirectorySeparatorChar + "spoiler.txt";
        if (!File.Exists(spoilerFile))
        {
            _logger.LogInformation("Could not find file {FilePath}", spoilerFile);
            _ttsService.Say(_trackerConfig.Responses.NoSeedFolderFound);
            return false;
        }
        ParseSpoilerLog(spoilerFile);
        return true;
    }

    private void ParseSpoilerLog(string path)
    {
        _logger.LogInformation("Parsing spoiler file {Path}", path);
        _locationItems.Clear();
        _itemLocations.Clear();
        _sealLocations.Clear();
        _npcLocations.Clear();
        _locationNpcs.Clear();
        var lines = File.ReadLines(path);
        var currentShopName = "";
        var parsingItems = false;
        var parsingShops = false;
        var parsingNpcs = false;
        var currentSeal = "";
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("Items:"))
            {
                _logger.LogInformation("Parsing items");
                parsingItems = true;
                parsingShops = false;
                parsingNpcs = false;
                currentSeal = "";
            }
            else if (trimmedLine.StartsWith("Shops:"))
            {
                _logger.LogInformation("Parsing shops");
                parsingItems = false;
                parsingShops = true;
                parsingNpcs = false;
                currentSeal = "";
            }
            else if (trimmedLine.StartsWith("NPCs:"))
            {
                _logger.LogInformation("Parsing npcs");
                parsingItems = false;
                parsingShops = false;
                parsingNpcs = true;
                currentSeal = "";
            }
            else if (trimmedLine is "Origin Seal:" or "Birth Seal:" or "Life Seal:" or "Death Seal:")
            {
                parsingItems = false;
                parsingShops = false;
                parsingNpcs = false;
                currentSeal = trimmedLine.Replace(":", "");
                _sealLocations[currentSeal] = new List<string>();
            }
            else if (trimmedLine is "Transitions:" or "Backside Doors:")
            {
                parsingItems = false;
                parsingShops = false;
                parsingNpcs = false;
                currentSeal = "";
            }
            else if (parsingItems && trimmedLine.Contains(" location"))
            {
                var parts = trimmedLine.Split(new char[] { ':' }, 2);
                var item = parts[0].Trim();
                var location = parts[1].Trim();
                AddSpoilerEntry(location, item);
            }
            else if (parsingShops && !trimmedLine.Contains("Item") && !string.IsNullOrEmpty(trimmedLine))
            {
                currentShopName = trimmedLine.Substring(0, trimmedLine.Length-1);
                _locationItems[currentShopName] = new List<string>();
            }
            else if (parsingShops && trimmedLine.Contains("Item") && !string.IsNullOrEmpty(trimmedLine))
            {
                var item = trimmedLine.Substring(7).Trim();
                AddSpoilerEntry(currentShopName, item);
            }
            else if (parsingNpcs && trimmedLine.Contains(" location"))
            {
                var parts = trimmedLine.Split(new char[] { ':' }, 2);
                var npc = parts[0].Trim();
                var location = parts[1].Trim();
                _locationNpcs[location] = npc;
                _npcLocations[npc] = location;
            }
            else if (!string.IsNullOrEmpty(currentSeal) && !string.IsNullOrEmpty(trimmedLine))
            {
                _sealLocations[currentSeal].Add(trimmedLine);
            }
        }
    }

    private void AddSpoilerEntry(string location, string item)
    {
        if (!_trackerConfig.Items.Contains(item))
        {
            _logger.LogWarning("{Item} from spoiler log not found in configs", item);
            return;
        }

        //_logger.LogInformation("{Item} is at {Location}", item, location);
        _itemLocations[item] = location;
        if (!_locationItems.ContainsKey(location))
        {
            _locationItems[location] = new List<string>();
        }
        _locationItems[location].Add(item);
    }

    public void EnableHints()
    {
        if (!LoadSpoilerLog())
        {
            return;
        }
        _hintsEnabled = true;
        _spoilersEnabled = false;
        _ttsService.Say(_trackerConfig.Responses.EnabledHints);
    }

    public void EnableSpoilers()
    {
        if (!LoadSpoilerLog())
        {
            return;
        }
        _hintsEnabled = false;
        _spoilersEnabled = true;
        _ttsService.Say(_trackerConfig.Responses.EnabledSpoilers);
    }

    public void Disable()
    {
        _hintsEnabled = false;
        _spoilersEnabled = false;
        _ttsService.Say(_trackerConfig.Responses.DisabledHintsAndSpoilers);
    }

    private void GiveItemHint(ItemConfig item)
    {
        var itemSpoilerName = item.SpoilerFileName;
        if (!_itemLocations.ContainsKey(itemSpoilerName))
        {
            _logger.LogWarning("Item {Item} ({SpoilerName}) not found in spoiler log", item.Key, item.SpoilerFileName);
            _ttsService.Say(_trackerConfig.Responses.ItemLocationNotFound);
            return;
        }

        var locationKey = _itemLocations[itemSpoilerName];
        var location = _trackerConfig.Locations.Get(locationKey);
        if (location == null)
        {
            _logger.LogWarning("Location {SpoilerName} from spoiler log not found in config file", locationKey);
            _ttsService.Say(_trackerConfig.Responses.HintItem, item.Names, locationKey);
            return;
        }

        if (_hintsEnabled)
        {
            if (!_hintsGiven.TryGetValue(item.Key, out var value))
            {
                value = 0;
            }
            if (location.Hints.Count <= value)
            {
                _ttsService.Say(_trackerConfig.Responses.NoHintsAvailable, item.Names);
                return;
            }

            _ttsService.Say(_trackerConfig.Responses.HintItem, item.Names, location.Hints[value]);
            _hintsGiven[item.Key] = value + 1;
        }
        else if (_spoilersEnabled)
        {
            _ttsService.Say(_trackerConfig.Responses.HintItem, item.Names, location.Names);
        }
    }
    
    private void GiveLocationHint(LocationConfig location)
    {
        if (!_locationItems.ContainsKey(location.Key))
        {
            _ttsService.Say(_trackerConfig.Responses.LocationHasUselessItem, location.Names);
            return;
        }

        var itemSpoilerNames = _locationItems[location.Key];
        var items = _trackerConfig.Items.Items.Where(x => itemSpoilerNames.Contains(x.SpoilerFileName)).ToList();
        if (items.Count == 0)
        {
            _ttsService.Say(_trackerConfig.Responses.LocationHasUselessItem, location.Names);
            return;
        }

        if (_hintsEnabled)
        {
            var hints = items.OrderBy(x => x.Key).SelectMany(i => i.Hints.Select(h => (i, h))).ToList();
            if (!_hintsGiven.TryGetValue(location.Key, out var value))
            {
                value = 0;
            }

            if (value < hints.Count)
            {
                _ttsService.Say(_trackerConfig.Responses.HintLocation, hints[value].h, location.Names);
                _hintsGiven[location.Key] = value + 1;
            }
            else if (value == hints.Count)
            {
                if (items.Any(x =>
                        x.Type is ItemType.Progression or ItemType.PotentiallyProgression or ItemType.AnkhJewel))
                {
                    _ttsService.Say(_trackerConfig.Responses.LocationHasProgressionItem, location.Names);    
                }
                else if (items.Any(x => x.Type is ItemType.NiceToHave or ItemType.NiceToHaveChecks))
                {
                    _ttsService.Say(_trackerConfig.Responses.LocationHasNiceToHaveItem, location.Names);
                }
                else
                {
                    _ttsService.Say(_trackerConfig.Responses.LocationHasUselessItem, location.Names);
                }
                
            }
            else
            {
                _ttsService.Say(_trackerConfig.Responses.NoHintsAvailable, location.Names);
            }

            
        }
        else if (_spoilersEnabled)
        {
            var itemNames = string.Join(" and ", items.Select(x => x.Names?.ToString() ?? x.SpoilerFileName));
            _ttsService.Say(_trackerConfig.Responses.HintLocation, itemNames, location.Names);
        }
    }
    
    private void GiveNpcHint(NpcConfig npc)
    {
        var npcKey = npc.Key;
        if (!_npcLocations.ContainsKey(npcKey))
        {
            _logger.LogWarning("NPC {Npc} not found in spoiler log", npcKey);
            _ttsService.Say(_trackerConfig.Responses.ItemLocationNotFound);
            return;
        }

        var locationKey = _npcLocations[npcKey];
        var location = _trackerConfig.NpcConfig.Locations.FirstOrDefault(x => x.Key == locationKey);
        if (location == null)
        {
            _logger.LogWarning("NPC location {SpoilerName} from spoiler log not found in config file", locationKey);
            _ttsService.Say(_trackerConfig.Responses.HintItem, npc.Names, locationKey);
            return;
        }

        if (_hintsEnabled)
        {
            if (!_hintsGiven.TryGetValue(npc.Key, out var value))
            {
                value = 0;
            }
            if (location.Hints.Count <= value)
            {
                _ttsService.Say(_trackerConfig.Responses.NoHintsAvailable, npc.Names);
                return;
            }

            _ttsService.Say(_trackerConfig.Responses.HintNpc, npc.Names, location.Hints[value]);
            _hintsGiven[npc.Key] = value + 1;
        }
        else if (_spoilersEnabled)
        {
            _ttsService.Say(_trackerConfig.Responses.HintNpc, npc.Names, location.Names);
        }
    }
    
    private void GiveNpcLocationHint(LocationConfig location)
    {
        if (!_locationNpcs.ContainsKey(location.Key))
        {
            _ttsService.Say(_trackerConfig.Responses.LocationHasUselessItem, location.Names);
            return;
        }

        var npcKey = _locationNpcs[location.Key];
        var npc = _trackerConfig.NpcConfig.Npcs.FirstOrDefault(x => x.Key == npcKey);
        if (npc == null)
        {
            _ttsService.Say(_trackerConfig.Responses.LocationHasUselessItem, location.Names);
            return;
        }

        if (_hintsEnabled)
        {
            if (!_hintsGiven.TryGetValue(location.Key, out var value))
            {
                value = 0;
            }
            
            if (npc.Hints.Count <= value)
            {
                _ttsService.Say(_trackerConfig.Responses.NoHintsAvailable, location.Names);
                return;
            }

            _ttsService.Say(_trackerConfig.Responses.HintNpcLocation, npc.Hints[value], location.Names);
            _hintsGiven[npc.Key] = value + 1;
        }
        else if (_spoilersEnabled)
        {
            _ttsService.Say(_trackerConfig.Responses.HintNpcLocation, npc.Names, location.Names);
        }
    }
    
    private void GiveRegionHint(RegionConfig region)
    {
        var items = _trackerConfig.Locations
            .Where(x => x.Region == region.Key)
            .SelectMany(x => _locationItems.TryGetValue(x.Key, out var item) ? item : new List<string>())
            .Select(x => _trackerConfig.Items.Get(x))
            .Where(x => x != null);

        if (!items.Any())
        {
            _ttsService.Say(_trackerConfig.Responses.RegionHasNothingUseful, region.Names);
            return;
        }

        if (_hintsEnabled)
        {
            var progressionCount =
                items.Count(x => x!.Type is ItemType.Progression or ItemType.PotentiallyProgression or ItemType.AnkhJewel);

            if (progressionCount > 0)
            {
                _ttsService.Say(_trackerConfig.Responses.RegionHasProgression, region.Names,
                    progressionCount == 1
                        ? "1 item"
                        : $"{progressionCount} items");
                return;
            }
            
            var niceToHaveCount =
                items.Count(x => x!.Type is ItemType.NiceToHave or ItemType.NiceToHaveChecks);
            if (niceToHaveCount > 0)
            {
                _ttsService.Say(_trackerConfig.Responses.RegionHasSomethingNice, region.Names,
                    niceToHaveCount == 1
                        ? "1 item"
                        : $"{progressionCount} items");
                return;
            }
            
            _ttsService.Say(_trackerConfig.Responses.RegionHasNothingUseful, region.Names);
        }
        else if (_spoilersEnabled)
        {
            var progressionItems =
                items.Where(x => x!.Type is ItemType.Progression or ItemType.PotentiallyProgression);
            var itemNames = string.Join(" and ", progressionItems.Select(x => x!.Names?.ToString() ?? x.SpoilerFileName));
            _ttsService.Say(_trackerConfig.Responses.RegionItems, region.Names, itemNames);
        }
    }
    
    private void GiveAnkhHints()
    {
        var ankhLocations = _trackerConfig.Items
            .Where(x => x.Type == ItemType.AnkhJewel)
            .Select(x => _itemLocations[x.SpoilerFileName])
            .Select(x => _trackerConfig.Locations.Get(x)!)
            .ToList();
        
        if (_hintsEnabled)
        {
            var badLocationRegions = ankhLocations.Where(x => _trackerConfig.Regions.Get(x.Region) == null)
                .Select(x => x.Region);
            
            var ankhRegionCounts = ankhLocations.Select(x => _trackerConfig.Regions.Get(x.Region)!)
                .GroupBy(x => x)
                .ToList();

            if (!ankhRegionCounts.Any())
            {
                _logger.LogWarning("Could not find any ankh jewels in the spoiler log");
                _ttsService.Say(_trackerConfig.Responses.ItemLocationNotFound);
            }
            
            if (!_hintsGiven.TryGetValue("ankh-jewels", out var value))
            {
                value = 0;
            }
            
            if (ankhRegionCounts.Count <= value)
            {
                _ttsService.Say(_trackerConfig.Responses.NoHintsAvailable, "ankh jewels");
                return;
            }

            var group = ankhRegionCounts[value];
            _ttsService.Say(_trackerConfig.Responses.HintAnkhJewel, group.Key.Names, group.Count() > 1 ? $"{group.Count()} ankh jewels" : "an ankh jewel");
            _hintsGiven["ankh-jewels"] = value + 1;
        }
        else if (_spoilersEnabled)
        {
            

            if (!ankhLocations.Any())
            {
                _logger.LogWarning("Could not find any ankh jewels in the spoiler log");
                _ttsService.Say(_trackerConfig.Responses.ItemLocationNotFound);
            }
            
            var locations = string.Join(" and ", ankhLocations.Select(x => x!.Names?.ToString() ?? x.Key));
            _ttsService.Say(_trackerConfig.Responses.AnkhJewelLocations, locations);
        }
    }
    
    private void GiveSealSpoiler(string seal)
    {
        if (!_sealLocations.ContainsKey(seal))
        {
            _ttsService.Say(_trackerConfig.Responses.ItemLocationNotFound);
        }

        var sealLocations = string.Join("; ", _sealLocations[seal]).Replace(",", "");
        var index = sealLocations.LastIndexOf(";", StringComparison.Ordinal);
        sealLocations = sealLocations.Insert(index + 2, "and ");
        
        _ttsService.Say(_trackerConfig.Responses.SpoilerSealLocations, seal, sealLocations);
    }
    
    
}