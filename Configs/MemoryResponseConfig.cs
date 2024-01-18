using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

public class MemoryResponseConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    public Dictionary<string, List<MemoryResponses>> MemoryResponses { get; set; } = new();

    public MemoryResponses? GetMemoryResponses(string memoryLocation, string currentValue, string previousValue, Dictionary<string, string> allMemory)
    {
        if (!MemoryResponses.TryGetValue(memoryLocation, out var valueResponses))
        {
            return null;
        }

        return valueResponses.FirstOrDefault(x =>
            x.Value == currentValue && (x.PreviousValue == null || x.PreviousValue == previousValue) && MatchesMemoryFilter(x, allMemory));
    }

    private static bool MatchesMemoryFilter(MemoryResponses response, Dictionary<string, string> allMemory)
    {
        if (string.IsNullOrWhiteSpace(response.FilterMemoryLocation) ||
            string.IsNullOrWhiteSpace(response.FilterMemoryValue))
        {
            return true;
        }
        
        allMemory.TryGetValue(response.FilterMemoryLocation!, out var otherValue);
        if (string.IsNullOrEmpty(otherValue))
        {
            otherValue = "0";
        }

        return otherValue == response.FilterMemoryValue;
    }
}