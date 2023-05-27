using System.Collections;
using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class TrackerConfig
{
    public ICollection<ItemConfig> Items { get; set; } = new List<ItemConfig>();
    public ResponseConfig Responses { get; set; } = new();
}