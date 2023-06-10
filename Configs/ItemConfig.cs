using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class ItemConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    public ItemType Type { get; set; } = ItemType.Unspecified;
    public bool IsMulti { get; set; }
    public SchrodingersString? Names { get; set; }
    public SchrodingersString? ArticledNames { get; set; }
    public SchrodingersString? PluralNames { get; set; }
    public SchrodingersString? OnTracked { get; set; }
    public string SpoilerFileName { get; set; } = "";
    public List<string> Hints { get; set; } = new();
}