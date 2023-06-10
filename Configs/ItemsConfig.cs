using System;
using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

public class ItemsConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    
    public List<ItemConfig> Items { get; set; } = new();
    
    public IEnumerable<ItemConfig> Where(Func<ItemConfig, bool> predicate) => Items.Where(predicate);
    
    public ItemConfig? FirstOrDefault(Func<ItemConfig, bool> predicate) => Items.FirstOrDefault(predicate);

    public ItemConfig? Get(string item) => Items.FirstOrDefault(x => x.Key == item || x.SpoilerFileName == item);
    public bool Contains(string item) => Items.Any(x => x.Key == item || x.SpoilerFileName == item);
}