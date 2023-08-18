using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class NpcsConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    
    public List<NpcConfig> Npcs { get; set; } = new();

    public List<LocationConfig> Locations { get; set; } = new();
}