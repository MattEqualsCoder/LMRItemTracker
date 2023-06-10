using System.Collections.Generic;
using LMRItemTracker.VoiceTracker;

namespace LMRItemTracker.Configs;

public class CustomPrompt : MergeableConfig
{
    public override string Key { get; set; } = "";

    public List<string> Prompts { get; set; } = new();

    public SchrodingersString Responses { get; set; } = new();

    public GrammarBuilder GetGrammar()
    {
        return new GrammarBuilder()
            .Append("Hey tracker, ")
            .OneOf(Prompts.ToArray());
    }
}