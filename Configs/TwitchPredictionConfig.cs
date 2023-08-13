using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class TwitchPredictionConfig : MergeableConfig
{
    public override string Key { get; set; }
    public List<string> StartPrompts { get; set; } = new();
    public List<string> ResolveGoodPrompts { get; set; } = new();
    public List<string> ResolveBadPrompts { get; set; } = new();
    public List<string> Prompts { get; set; } = new();
    public SchrodingersString StartResponses { get; set; } = new();
    public SchrodingersString PredictionTitles { get; set; } = new();
    public List<TwitchPredictionOption> PredictionOptions { get; set; } = new();
    public SchrodingersString ResolvedResponses { get; set; } = new();
}