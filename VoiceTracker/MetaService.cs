using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using LMRItemTracker.Configs;

namespace LMRItemTracker.VoiceTracker;

public class MetaService
{
    private readonly TextToSpeechService _tts;
    private ILogger<MetaService> _logger;
    private readonly Timer _timer;
    private readonly Random _random = new();
    private readonly TrackerConfig _config;

    public MetaService(VoiceRecognitionService voiceRecognitionService, TextToSpeechService tts, ConfigService configService, ILogger<MetaService> logger)
    {
        _tts = tts;
        _logger = logger;
        _config = configService.Config;
        _timer = new Timer(IdleTimerElasped, "idle", Timeout.Infinite, Timeout.Infinite);

        if (!configService.LoadedSuccessfully)
        {
            return;
        }

        voiceRecognitionService.AddCommand("shut up",
            new GrammarBuilder()
                .Append("Hey tracker, ")
                .OneOf("shut up", "stop talking"),
            result =>
            {
                _tts.StopTalking();
            }
        );
    }

    private void IdleTimerElasped(object? state)
    {
        var key = (string)state!;
        _tts.Say(_config.Responses.IdleMessage);
    }

    public void UpdateIdleTimer()
    {
        var span = (DateTime.Now.AddMinutes(8 + _random.NextDouble() * 4) - DateTime.Now);
        _timer.Change(span, Timeout.InfiniteTimeSpan);
    }
}
