using System.Collections.Generic;
using System.Threading.Tasks;
using Humanizer;
using LMRItemTracker.Configs;
using Microsoft.Extensions.Logging;

namespace LMRItemTracker.VoiceTracker;

public class TrackerService
{
    private readonly TextToSpeechService _ttsService;
    private readonly VoiceRecognitionService _voiceService;
    private readonly ILogger<TrackerService> _logger;
    private readonly TrackerConfig _config;
    private readonly ConfigService _configService;
    private readonly HintService _hintService;
    private readonly MetaService _metaService;
    private readonly Dictionary<string, int> _itemCounts = new();
    private bool _inGame;
    private bool _isWaitingOnStartGame;
    private bool _justDied;
    private bool _firstStart = true;

    public TrackerService(TextToSpeechService ttsService, VoiceRecognitionService voiceService, ConfigService configService, HintService hintService, MetaService metaService, ILogger<TrackerService> logger)
    {
        _ttsService = ttsService;
        _voiceService = voiceService;
        _logger = logger;
        _configService = configService;
        _config = configService.Config;
        _hintService = hintService;
        _metaService = metaService;
        
        if (!CanTrack)
        {
            return;
        }
        
        foreach (var customPrompt in _config.CustomPrompts.CustomPrompts)
        {
            voiceService.AddCommand(customPrompt.Key, customPrompt.GetGrammar(), result =>
            {
                _ttsService.Say(customPrompt.Responses);
            });
        }
        
        /*voiceService.AddCommand("test",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("initiate test"),
            result =>
            {
                _inGame = true;
                SetItemCount("ankh-jewels", 1);
                SetItemCount("ankh-jewels", 2);
                SetItemCount("ankh-jewels", 1);
                SetItemCount("ankh-jewels", 2);
            }
        );*/
        
        _logger.LogInformation("Voice tracker loaded");
    }

    public void Enable()
    {
        _ttsService.Muted = false;
        _voiceService.Enable();
        _logger.LogInformation("Voice tracker enabled");
    }

    public void Disable()
    {
        _ttsService.Muted = true;
        _voiceService.Disable();
        _logger.LogInformation("Voice tracker disabled");
    }

    public void UpdateRandomizerPath(string path)
    {
        _hintService.UpdateRandomizerPath(path);
    }

    public async void SetInGame(bool isInGame)
    {
        if (isInGame == _inGame || !CanTrack)
        {
            return;
        }
        
        if (isInGame && !_isWaitingOnStartGame && !_justDied)
        {
            if (_firstStart)
            {
                _ttsService.Say(_config.Responses.StartedGame);
                _firstStart = false;
                _metaService.UpdateIdleTimer();
            }
            _isWaitingOnStartGame = true;
            await Task.Delay(5 * 1000);
            _inGame = true;
            _isWaitingOnStartGame = false;
        }
        else if(!isInGame)
        {
            _inGame = false;
        }
    }

    public void SetItemState(string itemName, bool hasItem)
    {
        if (!CanTrack)
        {
            return;
        }
        var item = _config.Items.FirstOrDefault(x => x.Key == itemName);
        if (item == null)
        {
            _logger.LogWarning("Item {ItemName} not found", itemName);
            return;
        }
        _logger.LogInformation("Tracked {ItemName} | {Value}", itemName, hasItem);
        if (!hasItem || !_inGame) return;
        _metaService.UpdateIdleTimer();
        _ttsService.SayFallback(item.OnTracked, _config.Responses.BasicItemTracked, item.Names, item.ArticledNames);
    }
    
    public void SetItemCount(string itemName, int count)
    {
        if (!CanTrack)
        {
            return;
        }
        var item = _config.Items.FirstOrDefault(x => x.Key == itemName);
        if (item == null)
        {
            _logger.LogWarning("Item {ItemName} not found", itemName);
            return;
        }
        _logger.LogInformation("Tracked {ItemName} | {Count}", itemName, count);
        if (!_inGame)
        {
            return;
        }
        if (_itemCounts.ContainsKey(itemName) && count <= _itemCounts[itemName])
        {
            _itemCounts[itemName] = count;
            return;
        }
        _itemCounts[itemName] = count;
        _metaService.UpdateIdleTimer();
        if (item.IsMulti)
        {
            _ttsService.SayFallback(item.OnTracked, _config.Responses.MultiItemTracked.GetRollupResponse(count), item.Names, item.PluralNames, count, count.ToOrdinalWords());    
        }
        else
        {
            _ttsService.SayFallback(item.OnTracked, _config.Responses.BasicItemTracked, item.Names, item.ArticledNames);   
        }
    }

    public async void AddDeath(int amount)
    {
        if (!CanTrack)
        {
            return;
        }
        _justDied = true;
        _inGame = false;
        _metaService.UpdateIdleTimer();
        _logger.LogInformation("Tracked death {Value}", amount);
        _ttsService.Say(_config.Responses.PlayerDied.GetRollupResponse(amount), amount, amount.ToOrdinalWords());
        await Task.Delay(1 * 1000);
        _justDied = false;
    }

    public void SetBossState(string bossName, bool defeated)
    {
        if (!CanTrack)
        {
            return;
        }
        var boss = _config.Items.FirstOrDefault(x => x.Key == bossName);
        if (boss == null)
        {
            _logger.LogWarning("Boss {BossName} not found", bossName);
            return;
        }
        _logger.LogInformation("Tracked {BossName} | {Value}", bossName, defeated);
        if (!defeated || !_inGame) return;
        _metaService.UpdateIdleTimer();
        _ttsService.SayFallback(boss.OnTracked, _config.Responses.BossDefeated, boss.Names);
        
    }

    public bool CanTrack => _configService.LoadedSuccessfully;
}