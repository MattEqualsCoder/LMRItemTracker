using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
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
    private readonly Dictionary<string, bool> _regionStates = new();
    private readonly Dictionary<string, bool> _bosses = new();
    private List<string> _previousLastItems = new();
    private List<Action> _undoActions = new();
    private bool _inGame;
    private bool _isWaitingOnStartGame;
    private bool _justDied;
    private bool _firstStart = true;
    private int _countToResetLastItems = 0;

    public TrackerService(TextToSpeechService ttsService, VoiceRecognitionService voiceService, ConfigService configService, HintService hintService, MetaService metaService, ILogger<TrackerService> logger, ChoiceService choices)
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

        ResetRegions();
        
        voiceService.AddCommand("item hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("clear the recent items", "clear recent items"),
            result =>
            {
                if (TrackerForm == null)
                {
                    return;
                }

                _countToResetLastItems = 3;
                _previousLastItems.AddRange(TrackerForm.LastItems);
                TrackerForm.ClearRecentItems();
                _ttsService.Say(_config.Responses.ClearedRecentItems);
            }
        );

        voiceService.AddCommand("clear region",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("clear", "clear all locations from")
                .Append(choices.RegionKey, choices.GetRegionNames()),
            result =>
            {
                var regionConfig = choices.GetRegionFromResult(result, out string locationName);
                SetRegionState(regionConfig, true);
            }
        );

        voiceService.AddCommand("clear region 2",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("mark", "set")
                .Append(choices.RegionKey, choices.GetRegionNames())
                .OneOf("as all cleared"),
            result =>
            {
                var regionConfig = choices.GetRegionFromResult(result, out string locationName);
                SetRegionState(regionConfig, true);
            }
        );

        voiceService.AddCommand("region hints",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("unclear", "unclear all locations from")
                .Append(choices.RegionKey, choices.GetRegionNames()),
            result =>
            {
                var regionConfig = choices.GetRegionFromResult(result, out string locationName);
                SetRegionState(regionConfig, false);
            }
        );

        voiceService.AddCommand("clear region 2",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("mark", "set")
                .Append(choices.RegionKey, choices.GetRegionNames())
                .OneOf("as not cleared"),
            result =>
            {
                var regionConfig = choices.GetRegionFromResult(result, out string locationName);
                SetRegionState(regionConfig, false);
            }
        );

        voiceService.AddCommand("undo",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("undo that", "control z"),
            result =>
            {
                if (_undoActions.Any())
                {
                    _ttsService.Say(_config.Responses.Undo);
                    _undoActions[0].Invoke();
                    _undoActions.RemoveAt(0);
                }
            }
        );

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

    public void ResetRegions()
    {
        foreach (var region in _config.Regions.Regions)
        {
            _regionStates[region.Key] = false;
            TrackerForm?.UpdateRegion(region.Key, false);
        }
    }
    
    public LaMulanaItemTrackerForm? TrackerForm { get; set; }

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

        if (_previousLastItems.Contains(itemName))
        {
            _ttsService.Say(_config.Responses.GotClearedItem);
        }
        else if (_countToResetLastItems > 0)
        {
            _countToResetLastItems--;
            if (_countToResetLastItems <= 0)
            {
                _previousLastItems.Clear();
            }
        }
        
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
        if (_bosses.ContainsKey(bossName) && _bosses[bossName] == defeated)
        {
            return;
        }
        _bosses[bossName] = defeated;
        _logger.LogInformation("Tracked {BossName} | {Value}", bossName, defeated);
        if (!defeated || !_inGame) return;
        _metaService.UpdateIdleTimer();
        _ttsService.SayFallback(boss.OnTracked, _config.Responses.BossDefeated, boss.Names);
        
    }

    private void SetRegionState(RegionConfig region, bool isCleared, bool addUndo = true)
    {
        if (_regionStates[region.Key] == isCleared || TrackerForm == null)
        {
            return;
        }

        _regionStates[region.Key] = isCleared;
        TrackerForm?.UpdateRegion(region.Key, isCleared);

        if (isCleared)
        {
            _ttsService.Say(_config.Responses.ClearedRegion, region.Names);
        }
        else
        {
            _ttsService.Say(_config.Responses.UnclearedRegion, region.Names);
        }

        if (addUndo)
        {
            AddUndoAction(new Action(() =>
            {
                SetRegionState(region, !isCleared, false);
            }));
        }
        
    }

    private void AddUndoAction(Action action)
    {
        _undoActions.Insert(0, action);

        if (_undoActions.Count > 5)
        {
            _undoActions.RemoveAt(5);
        }
    }

    public bool CanTrack => _configService.LoadedSuccessfully;
}