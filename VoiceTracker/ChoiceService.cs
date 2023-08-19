using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using LMRItemTracker.Configs;

namespace LMRItemTracker.VoiceTracker;

public class ChoiceService
{
    private TrackerConfig _trackerConfig;
    
    private readonly List<string> _sealChoices = new List<string>()
        { "Origin Seal", "Birth Seal", "Life Seal", "Death Seal" };
    
    private const string ItemKeyValue = "ItemKey";
    private const string LocationKeyValue = "LocationKey";
    private const string RegionKeyValue = "RegionKey";
    private const string PredictionKeyValue = "PredictionKey";
    private const string NpcKeyValue = "NpcKey";
    private const string SealKeyValue = "SealKey";

    public string ItemKey => ItemKeyValue;
    public string LocationKey => LocationKeyValue;
    public string RegionKey => RegionKeyValue;
    public string PredictionKey => PredictionKeyValue;
    public string NpcKey => NpcKeyValue;
    public string SealKey => SealKeyValue;
    

    public ChoiceService(ConfigService configService)
    {
        _trackerConfig = configService.Config;
    }
    
    public Choices GetItemNames()
    {
        var itemNames = new Choices();
        
        foreach (var item in _trackerConfig.Items.Where(x => x.Names != null && !string.IsNullOrWhiteSpace(x.SpoilerFileName)))
        {
            foreach (var name in item.Names!)
            {
                itemNames.Add(new SemanticResultValue(name.ToString(), item.Key));
            }
        }

        return itemNames;
    }
    
    public Choices GetLocationNames()
    {
        var locationNames = new Choices();
        
        foreach (var location in _trackerConfig.Locations.Where(x => x.Names != null))
        {
            foreach (var name in location.Names!)
            {
                locationNames.Add(new SemanticResultValue(name.ToString(), location.Key));
            }
        }

        return locationNames;
    }
    
    public Choices GetRegionNames()
    {
        var regionNames = new Choices();
        
        foreach (var region in _trackerConfig.Regions.Where(x => x.Names != null))
        {
            foreach (var name in region.Names!)
            {
                regionNames.Add(new SemanticResultValue(name.ToString(), region.Key));
            }
        }

        return regionNames;
    }
    
    public Choices GetStartPredictionNames()
    {
        var choices = new Choices();
        
        foreach (var prediction in _trackerConfig.TwitchConfig.Predictions)
        {
            foreach (var name in prediction.StartPrompts)
            {
                choices.Add(new SemanticResultValue(name, prediction.Key));
            }
        }

        return choices;
    }
    
    public Choices GetResolveGoodPredictionNames()
    {
        var choices = new Choices();
        
        foreach (var prediction in _trackerConfig.TwitchConfig.Predictions)
        {
            foreach (var name in prediction.ResolveGoodPrompts)
            {
                choices.Add(new SemanticResultValue(name, prediction.Key));
            }
        }

        return choices;
    }
    
    public Choices GetResolveBadPredictionNames()
    {
        var choices = new Choices();
        
        foreach (var prediction in _trackerConfig.TwitchConfig.Predictions)
        {
            foreach (var name in prediction.ResolveBadPrompts)
            {
                choices.Add(new SemanticResultValue(name, prediction.Key));
            }
        }

        return choices;
    }
    
    public Choices GetNpcNames()
    {
        var choices = new Choices();
        
        foreach (var npc in _trackerConfig.NpcConfig.Npcs.Where(x => x.Names != null))
        {
            foreach (var name in npc.Names!)
            {
                choices.Add(new SemanticResultValue(name, npc.Key));
            }
        }

        return choices;
    }
    
    public Choices GetNpcLocationNames()
    {
        var choices = new Choices();
        
        foreach (var location in _trackerConfig.NpcConfig.Locations.Where(x => x.Names != null))
        {
            foreach (var name in location.Names!)
            {
                choices.Add(new SemanticResultValue(name, location.Key));
            }
        }

        return choices;
    }
    
    public Choices GetSealNames()
    {
        var choices = new Choices();
        
        foreach (var seal in _sealChoices)
        {
            choices.Add(new SemanticResultValue(seal, seal));
        }

        return choices;
    }
    
    public ItemConfig GetItemFromResult(RecognitionResult result, out string itemName)
    {
        itemName = (string)result.Semantics[ItemKey].Value;
        var key = itemName;
        var item = _trackerConfig.Items.FirstOrDefault(x => x.Key == key);
        return item ?? throw new KeyNotFoundException($"Could not find recognized item '{itemName}' (\"{result.Text}\")");
    }
    
    public LocationConfig GetLocationFromResult(RecognitionResult result, out string locationName)
    {
        locationName = (string)result.Semantics[LocationKey].Value;
        var key = locationName;
        var location = _trackerConfig.Locations.FirstOrDefault(x => x.Key == key);
        return location ?? throw new KeyNotFoundException($"Could not find recognized item '{locationName}' (\"{result.Text}\")");
    }
    
    public RegionConfig GetRegionFromResult(RecognitionResult result, out string regionName)
    {
        regionName = (string)result.Semantics[RegionKey].Value;
        var key = regionName;
        var region = _trackerConfig.Regions.FirstOrDefault(x => x.Key == key);
        return region ?? throw new KeyNotFoundException($"Could not find recognized item '{regionName}' (\"{result.Text}\")");
    }
    
    public TwitchPredictionConfig GetPredictionConfigFromResult(RecognitionResult result)
    {
        var key = (string)result.Semantics[PredictionKey].Value;
        var prediction = _trackerConfig.TwitchConfig.Predictions.FirstOrDefault(x => x.Key == key);
        return prediction ?? throw new KeyNotFoundException($"Could not find recognized prediction '{key}' (\"{result.Text}\")");
    }
    
    public NpcConfig GetNpcConfigFromResult(RecognitionResult result)
    {
        var key = (string)result.Semantics[NpcKey].Value;
        var npc = _trackerConfig.NpcConfig.Npcs.FirstOrDefault(x => x.Key == key);
        return npc ?? throw new KeyNotFoundException($"Could not find recognized prediction '{key}' (\"{result.Text}\")");
    }
    
    public LocationConfig GetNpcLocationConfigFromResult(RecognitionResult result)
    {
        var key = (string)result.Semantics[LocationKey].Value;
        var location = _trackerConfig.NpcConfig.Locations.FirstOrDefault(x => x.Key == key);
        return location ?? throw new KeyNotFoundException($"Could not find recognized prediction '{key}' (\"{result.Text}\")");
    }
    
    public string GetSealFromResult(RecognitionResult result)
    {
        return (string)result.Semantics[SealKey].Value;
    }
}