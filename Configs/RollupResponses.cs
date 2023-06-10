using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

public class RollupResponses : Dictionary<int, SchrodingersString>
{
    public SchrodingersString GetRollupResponse(int value)
    {
        var key = Keys.Where(x => x <= value).OrderBy(x => x).LastOrDefault();
        return this[key];
    }

    public void Merge(RollupResponses other)
    {
        foreach (var key in Keys.Where(x => other.Keys.Contains(x)))
        {
            this[key].Merge(other[key]);
        }

        foreach (var key in other.Keys.Where(x => !Keys.Contains(x)))
        {
            this[key] = other[key];
        }
    }
}