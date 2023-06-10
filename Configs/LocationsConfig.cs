using System;
using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

public class LocationsConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    
    public List<LocationConfig> Locations { get; set; } = new();

    public IEnumerable<LocationConfig> Where(Func<LocationConfig, bool> predicate) => Locations.Where(predicate);
    
    public LocationConfig? FirstOrDefault(Func<LocationConfig, bool> predicate) => Locations.FirstOrDefault(predicate);

    public LocationConfig? Get(string location) => FirstOrDefault(x => x.Key == location);
    
    public bool Contains(string location) => Locations.Any(x => x.Key == location);
}