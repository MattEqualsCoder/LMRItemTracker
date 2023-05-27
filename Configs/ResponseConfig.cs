namespace LMRItemTracker.Configs;

public class ResponseConfig
{
    public SchrodingersString BasicItemTracked { get; set; } = new("Tracked {0}");
    public SchrodingersString BossDefeated { get; set; } = new("Defeated {0}");
    public SchrodingersString MultiStepItemTracked { get; set; } = new("Tracked {0}");
    public SchrodingersString PlayerDied { get; set; } = new("Tracked death");
}