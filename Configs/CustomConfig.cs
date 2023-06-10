using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class CustomConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    public List<CustomPrompt> CustomPrompts { get; set; } = new();
}