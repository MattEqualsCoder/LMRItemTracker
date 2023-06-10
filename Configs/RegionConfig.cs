using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class RegionConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    public SchrodingersString? Names { get; set; } = new();
    public List<string> Hints { get; set; } = new();
}