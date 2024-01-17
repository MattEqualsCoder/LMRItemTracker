using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class BossConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    public int KilledValue { get; set; } = 1;
    public SchrodingersString? Names { get; set; }
    public SchrodingersString? ArticledNames { get; set; }
    public SchrodingersString? PluralNames { get; set; }
    public SchrodingersString? OnTracked { get; set; }
    
}