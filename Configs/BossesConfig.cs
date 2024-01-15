using System;
using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

public class BossesConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    
    public List<BossConfig> Bosses { get; set; } = new();
    
    public IEnumerable<BossConfig> Where(Func<BossConfig, bool> predicate) => Bosses.Where(predicate);
    
    public BossConfig? FirstOrDefault(Func<BossConfig, bool> predicate) => Bosses.FirstOrDefault(predicate);

    public BossConfig? Get(string item) => Bosses.FirstOrDefault(x => x.Key == item);
    public bool Contains(string item) => Bosses.Any(x => x.Key == item);
}