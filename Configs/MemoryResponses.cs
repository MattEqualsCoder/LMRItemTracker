namespace LMRItemTracker.Configs;

public class MemoryResponses : MergeableConfig
{
    public override string Key { get; set; } = "";
    public string Value { get; set; } = "";
    public string? PreviousValue { get; set; }
    public bool SayOnce { get; set; }
    public string? FilterMemoryLocation { get; set; }
    public string? FilterMemoryValue { get; set; }
    public SchrodingersString Responses { get; set; } = new();
    public SchrodingersString? ResponsesOnRepeat { get; set; }
}