using System.Linq;
using LMRItemTracker.Configs;
using Microsoft.Extensions.Logging;

namespace LMRItemTracker.VoiceTracker;

public class TrackerService
{
    private TextToSpeechService _ttsService;
    private VoiceRecognitionService _voiceService;
    private ILogger<TrackerService> _logger;
    private TrackerConfig _config;
    
    public TrackerService(TextToSpeechService ttsService, VoiceRecognitionService voiceService, ConfigService configService, ILogger<TrackerService> logger)
    {
        _ttsService = ttsService;
        _voiceService = voiceService;
        _logger = logger;
        _config = configService.Config;
    }

    public void Say(SchrodingersString? text, params object?[] args)
    {
        if (text == null)
        {
            return;
        }
        
        var line = text.Format(args);
        if (!string.IsNullOrEmpty(line))
        {
            _ttsService.Say(line!);    
        }
    }

    public void SetItemState(string itemName, bool hasItem)
    {
        if (!hasItem) return;
        var item = _config.Items.FirstOrDefault(x => x.Key == itemName);
        if (item == null)
        {
            _logger.LogWarning("Item {ItemName} not found", itemName);
            return;
        }
        _logger.LogInformation("Tracked {ItemName}", itemName);
        Say(_config.Responses.BasicItemTracked, item.Names);
    }
    
    public void SetItemStepState(string itemName, string stepName, int stepNumber, bool isAdd)
    {
        if (!isAdd) return;
        var item = _config.Items.FirstOrDefault(x => x.Key == itemName);
        if (item == null)
        {
            _logger.LogWarning("Item {ItemName} not found", itemName);
            return;
        }
        _logger.LogInformation("Tracked {ItemName}", itemName);
        Say(_config.Responses.MultiStepItemTracked, item.Names);
    }

    public void AddDeath(int amount)
    {
        _logger.LogInformation("Tracked death");
        Say(_config.Responses.PlayerDied, amount);
    }

    public void SetBossState(string bossName, bool defeated)
    {
        if (!defeated) return;
        var boss = _config.Items.FirstOrDefault(x => x.Key == bossName);
        if (boss == null)
        {
            _logger.LogWarning("Boss {BossName} not found", bossName);
            return;
        }
        _logger.LogInformation("Defeated {ItemName}", bossName);
        Say(_config.Responses.BossDefeated, boss.Names);
        
    }
}