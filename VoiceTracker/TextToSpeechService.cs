using LMRItemTracker.Configs;
using System;
using System.Speech.Synthesis;

namespace LMRItemTracker.VoiceTracker;

public class TextToSpeechService : IDisposable
{
    private readonly SpeechSynthesizer _tts;
    
    public TextToSpeechService()
    {
        _tts = new SpeechSynthesizer();
        _tts.SelectVoiceByHints(VoiceGender.Female);
    }
    
    public void Dispose()
    {
        _tts.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool Muted { get; set; }
    
    public void Say(string text)
    {
        if (!Muted && !string.IsNullOrWhiteSpace(text))
        {
            _tts.Speak(text);
        }
    }

    public bool Say(SchrodingersString? text, params object?[] args)
    {
        if (text == null)
        {
            return false;
        }

        var line = text.Format(args);
        if (string.IsNullOrWhiteSpace(line)) return false;
        Say(line!);
        return true;
    }
    
    public void SayFallback(SchrodingersString? primary, SchrodingersString? secondary, params object?[] args)
    {
        if (!Say(primary, args))
        {
            Say(secondary, args);
        }
    }

    public void StopTalking()
    {
        _tts.SpeakAsyncCancelAll();
    }
}