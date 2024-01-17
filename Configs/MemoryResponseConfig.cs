using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

public class MemoryResponseConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    public Dictionary<string, List<MemoryResponses>> MemoryResponses { get; set; } = new();

    public MemoryResponses? GetMemoryResponses(string memoryLocation, string currentValue, string previousValue)
    {
        if (!MemoryResponses.TryGetValue(memoryLocation, out var valueResponses))
        {
            return null;
        }

        return valueResponses.FirstOrDefault(x =>
            x.Value == currentValue && (x.PreviousValue == null || x.PreviousValue == previousValue));
    }
}