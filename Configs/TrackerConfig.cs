using System.Collections;
using System.Collections.Generic;

namespace LMRItemTracker.Configs;

public class TrackerConfig
{
    public ItemsConfig Items { get; set; } = new();
    public ResponseConfig Responses { get; set; } = new();
    public CustomConfig CustomPrompts { get; set; } = new();
    public LocationsConfig Locations { get; set; } = new();
    public RegionsConfig Regions { get; set; } = new();
    public TwitchConfig TwitchConfig { get; set; } = new();
    
    public NpcsConfig NpcConfig { get; set; } = new();

    public TrackerConfig(ItemsConfig? items, LocationsConfig? locations, RegionsConfig? regions, ResponseConfig? responses,
        CustomConfig? customConfig, TwitchConfig? twitchConfig, NpcsConfig? npcConfig)
    {
        if (items != null)
            Items = items;
        if (locations != null)
            Locations = locations;
        if (regions != null)
            Regions = regions;
        if (responses != null)
            Responses = responses;
        if (customConfig != null)
            CustomPrompts = customConfig;
        if (twitchConfig != null)
            TwitchConfig = twitchConfig;
        if (npcConfig != null)
            NpcConfig = npcConfig;
    }

    public void Merge(TrackerConfig other)
    {
        Items.Merge(other.Items);
        Responses.Merge(other.Responses);
        CustomPrompts.Merge(other.CustomPrompts);
        Locations.Merge(other.Locations);
        Regions.Merge(other.Regions);
        TwitchConfig.Merge(other.TwitchConfig);
        NpcConfig.Merge(other.NpcConfig);
    }
}