using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LMRItemTracker.Configs;

/// <summary>
/// Interface for classes where the data can be merged
/// between two instances of the object
/// </summary>
/// <typeparam name="T">
/// The type of data to be merged. By default, if the type
/// matches the class it will merge the properties. If the
/// type does not match the class, it will treat it like a
/// list of that other type.
/// </typeparam>
public abstract class MergeableConfig
{
    public abstract string Key { get; set; }


    public static void Merge<T>(ICollection<T> primary,
        ICollection<T> secondary) where T : MergeableConfig
    {
        foreach (var primaryObj in primary)
        {
            var secondaryObj = secondary.FirstOrDefault(s => s.Key == primaryObj.Key);
            if (secondaryObj != null)
            {
                primaryObj.Merge(secondaryObj);
            }
        }

        var primaryKeys = primary.Select(p => p.Key).ToHashSet();
        foreach (var secondaryObj in secondary.Where(s => !primaryKeys.Contains(s.Key)))
        {
            primary.Add(secondaryObj);
        }
    }
    
    /// <summary>
    /// Merges the data from the other object into the current instance
    /// </summary>
    /// <param name="other">The object to be merged into this one</param>
    public void Merge(MergeableConfig other)
    {
        if (GetType() != other.GetType())
        {
            throw new ArgumentException(GetType().Name + " cannot be merged with " + other.GetType().Name);
        }

        var properties = GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(Dictionary<int, SchrodingersString>))
            {
                var thisValue = (Dictionary<int, SchrodingersString>?)property.GetValue(this);
                var otherValue = (Dictionary<int, SchrodingersString>?)property.GetValue(other);

                if (thisValue != null && otherValue != null)
                {
                    foreach (var otherData in otherValue)
                    {
                        if (thisValue.ContainsKey(otherData.Key) && thisValue[otherData.Key] != null)
                        {
                            thisValue[otherData.Key].Merge(otherData.Value);
                        }
                        else
                        {
                            thisValue[otherData.Key] = otherData.Value;
                        }
                    }
                }
                else if (thisValue == null)
                {
                    property.SetValue(this, otherValue);
                }
            }
            else if (property.PropertyType == typeof(Dictionary<string, SchrodingersString>))
            {
                var thisValue = (Dictionary<string, SchrodingersString>?)property.GetValue(this);
                var otherValue = (Dictionary<string, SchrodingersString>?)property.GetValue(other);

                if (thisValue != null && otherValue != null)
                {
                    foreach (var otherData in otherValue)
                    {
                        if (thisValue.ContainsKey(otherData.Key) && thisValue[otherData.Key] != null)
                        {
                            thisValue[otherData.Key].Merge(otherData.Value);
                        }
                        else
                        {
                            thisValue[otherData.Key] = otherData.Value;
                        }
                    }
                }
                else if (thisValue == null)
                {
                    property.SetValue(this, otherValue);
                }
            }
            else if (property.PropertyType == typeof(Dictionary<string, string>))
            {
                var thisValue = (Dictionary<string, string>?)property.GetValue(this);
                var otherValue = (Dictionary<string, string>?)property.GetValue(other);

                if (thisValue == null)
                {
                    property.SetValue(this, otherValue);
                }
                else if (otherValue != null)
                {
                    foreach (var otherData in otherValue)
                    {
                        thisValue[otherData.Key] = otherData.Value;
                    }
                }
            }
            else if (property.PropertyType == typeof(List<string>))
            {
                var thisValue = (List<string>?)property.GetValue(this);
                var otherValue = (List<string>?)property.GetValue(other);

                if (thisValue != null && otherValue != null)
                {
                    thisValue.AddRange(otherValue.Where(x => !thisValue.Contains(x)));
                }
                else if (thisValue == null)
                {
                    property.SetValue(this, otherValue);
                }
            }
            else if (property.PropertyType.IsAssignableFrom(typeof(MergeableConfig)))
            {
                var thisValue = property.GetValue(this) as MergeableConfig;
                var otherValue = property.GetValue(other) as MergeableConfig;

                if (thisValue != null && otherValue != null)
                {
                    thisValue.Merge(otherValue);
                }
                else if (thisValue == null)
                {
                    property.SetValue(this, otherValue);
                }
            }
            else if (property.PropertyType == typeof(SchrodingersString))
            {
                var thisValue = property.GetValue(this) as SchrodingersString;
                var otherValue = property.GetValue(other) as SchrodingersString;
                
                if (thisValue != null && otherValue != null)
                {
                    thisValue.Merge(otherValue);    
                }
                else if (thisValue == null)
                {
                    property.SetValue(this, otherValue);
                }
            }
            else if (property.PropertyType == typeof(List<ItemConfig>))
            {
                var thisValue = property.GetValue(this) as List<ItemConfig>;
                var otherValue = property.GetValue(other) as List<ItemConfig>;
                Merge(thisValue ?? new List<ItemConfig>(), otherValue ?? new List<ItemConfig>());
            }
            else if (property.PropertyType == typeof(List<LocationConfig>))
            {
                var thisValue = property.GetValue(this) as List<LocationConfig>;
                var otherValue = property.GetValue(other) as List<LocationConfig>;
                Merge(thisValue ?? new List<LocationConfig>(), otherValue ?? new List<LocationConfig>());
            }
            else if (property.PropertyType == typeof(List<RegionConfig>))
            {
                var thisValue = property.GetValue(this) as List<RegionConfig>;
                var otherValue = property.GetValue(other) as List<RegionConfig>;
                Merge(thisValue ?? new List<RegionConfig>(), otherValue ?? new List<RegionConfig>());
            }
            else if (property.PropertyType == typeof(List<CustomPrompt>))
            {
                var thisValue = property.GetValue(this) as List<CustomPrompt>;
                var otherValue = property.GetValue(other) as List<CustomPrompt>;
                Merge(thisValue ?? new List<CustomPrompt>(), otherValue ?? new List<CustomPrompt>());
            }
            else if (property.PropertyType == typeof(List<TwitchPredictionConfig>))
            {
                var thisValue = property.GetValue(this) as List<TwitchPredictionConfig>;
                var otherValue = property.GetValue(other) as List<TwitchPredictionConfig>;
                Merge(thisValue ?? new List<TwitchPredictionConfig>(), otherValue ?? new List<TwitchPredictionConfig>());
            }
            else if (property.PropertyType == typeof(List<NpcConfig>))
            {
                var thisValue = property.GetValue(this) as List<NpcConfig>;
                var otherValue = property.GetValue(other) as List<NpcConfig>;
                Merge(thisValue ?? new List<NpcConfig>(), otherValue ?? new List<NpcConfig>());
            }
            else if (property.PropertyType == typeof(RollupResponses))
            {
                var thisValue = property.GetValue(this) as RollupResponses;
                var otherValue = property.GetValue(other) as RollupResponses;
                
                if (thisValue != null && otherValue != null)
                {
                    thisValue.Merge(otherValue);    
                }
                else if (thisValue == null)
                {
                    property.SetValue(this, otherValue);
                }
            }
        }
    }
}