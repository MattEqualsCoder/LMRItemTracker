namespace LMRItemTracker.Configs;

public class TwitchPredictionOption : MergeableConfig
{
    public override string Key
    {
        get => Good + "_" + Bad;
        set => _ = value;
    }

    public string Good { get; set; } = "";
    public string Bad { get; set; } = "";
}