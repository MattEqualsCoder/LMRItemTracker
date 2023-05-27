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
    
    public void Say(string text)
    {
        _tts.Speak(text);
    }
}