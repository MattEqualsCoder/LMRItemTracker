namespace LMRItemTracker.Configs;

public class ResponseConfig : MergeableConfig
{
    public override string Key { get; set; } = "";
    
    public SchrodingersString BasicItemTracked { get; set; } = new();
    public SchrodingersString BossDefeated { get; set; } = new();
    public RollupResponses MultiItemTracked { get; set; } = new();
    public RollupResponses PlayerDied { get; set; } = new();
    public SchrodingersString StartedGame { get; set; } = new();
    public SchrodingersString EnabledHints { get; set; } = new();
    public SchrodingersString EnabledSpoilers { get; set; } = new();
    public SchrodingersString DisabledHintsAndSpoilers { get; set; } = new();
    public SchrodingersString NoHintsAvailable { get; set; } = new();
    public SchrodingersString HintItem { get; set; } = new();
    public SchrodingersString HintLocation { get; set; } = new();
    public SchrodingersString NeedToEnableHints { get; set; } = new();
    public SchrodingersString LocationHasProgressionItem { get; set; } = new();
    public SchrodingersString LocationHasNiceToHaveItem { get; set; } = new();
    public SchrodingersString LocationHasUselessItem { get; set; } = new();
    public SchrodingersString RegionHasNothingUseful { get; set; } = new();
    public SchrodingersString RegionHasProgression { get; set; } = new();
    public SchrodingersString RegionHasSomethingNice { get; set; } = new();
    public SchrodingersString RegionItems { get; set; } = new();
    public SchrodingersString HintAnkhJewel { get; set; } = new();
    public SchrodingersString AnkhJewelLocations { get; set; } = new();
    public SchrodingersString UnrecognizedLine { get; set; } = new();
    public SchrodingersString IdleMessage { get; set; } = new();
    public string NoSeedFolderFound = "No valid randomized seed folder found";
    public string NoRandomizerPath = "No randomizer path specifield"; 
    public string InvalidRandomizerPath = "No randomizer jar file found in directory";
    public string ItemLocationNotFound = "I could not find that in the spoiler log";
    public string Error = "Sorry, there was an error with that. Please send the log to MattEqualsCoder.";

}