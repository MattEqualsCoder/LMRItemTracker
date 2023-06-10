using System;
using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

public class RegionsConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    
    public List<RegionConfig> Regions { get; set; } = new();

    public IEnumerable<RegionConfig> Where(Func<RegionConfig, bool> predicate) => Regions.Where(predicate);
    
    public RegionConfig? FirstOrDefault(Func<RegionConfig, bool> predicate) => Regions.FirstOrDefault(predicate);

    public RegionConfig? Get(string region) => FirstOrDefault(x => x.Key == region);
    
    public bool Contains(string region) => Regions.Any(x => x.Key == region);
}