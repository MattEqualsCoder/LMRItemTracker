using System;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using Microsoft.Extensions.Logging;

namespace LMRItemTracker.VoiceTracker;

public class VoiceRecognitionService
{
    private SpeechRecognitionEngine _speechRecognitionEngine;
    private ILogger<VoiceRecognitionService> _logger;
    
    /// <summary>
    /// Dll to get the number of microphones
    /// </summary>
    /// <returns></returns>
    [DllImport("winmm.dll")]
    private static extern int waveInGetNumDevs();

    public VoiceRecognitionService(ILogger<VoiceRecognitionService> logger)
    {
        _logger = logger;
        _speechRecognitionEngine = new SpeechRecognitionEngine();
        _speechRecognitionEngine.SpeechRecognized += (sender, args) =>
        {
            _logger.LogInformation("Test");
        };
        _speechRecognitionEngine.SpeechDetected += (sender, args) =>
        {
            _logger.LogInformation("Test2");
        };
        _speechRecognitionEngine.LoadGrammarCompleted += (sender, args) =>
        {
            _logger.LogInformation("Grammar Loaded Complete");
        };
        _logger.LogInformation("Done");
    }

    public bool Enable()
    {
        _logger.LogInformation("Starting speech recognition");
        try
        {
            if (waveInGetNumDevs() == 0)
            {
                _logger.LogWarning("No microphone");
                return false;
            }
            
           // _speechRecognitionEngine.EmulateRecognize("Hey Tracker, good morning");
            _speechRecognitionEngine.SetInputToDefaultAudioDevice();
            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            _logger.LogInformation("Speech recognition started");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error initializing microphone");
            return false;
        }
    }

    public void Disable()
    {
        _logger.LogInformation("Stopping speech recognition");
        _speechRecognitionEngine.RecognizeAsyncStop();
    }

    public void AddCommand(string ruleName, GrammarBuilder2 grammarBuilder, Action<RecognitionResult> command)
    {
        try
        {
            var grammar = grammarBuilder.Build(ruleName);
            grammar.SpeechRecognized += (sender, e) =>
            {
                _logger.LogInformation("Recognized \"{Text}\" with {Confidence:P2} confidence", e.Result.Text, e.Result.Confidence);
                command(e.Result);
            };
            _speechRecognitionEngine.LoadGrammar(grammar);
            _logger.LogInformation("Grammar Added");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "An error occurred while constructing the speech recognition engine");
            throw;
        }
    }
    
    /// <summary>
    /// Adds a new voice command that matches the specified phrase.
    /// </summary>
    /// <param name="ruleName">The name of the command.</param>
    /// <param name="phrase">The phrase to recognize.</param>
    /// <param name="executeCommand">
    /// The command to execute when the phrase is recognized.
    /// </param>
    public void AddCommand(string ruleName, string phrase,
        Action<RecognitionResult> executeCommand)
    {
        var builder = new GrammarBuilder2()
            .Append(phrase);

        AddCommand(ruleName, builder, executeCommand);
    }

    /// <summary>
    /// Adds a new voice command that matches any of the specified phrases.
    /// </summary>
    /// <param name="ruleName">The name of the command.</param>
    /// <param name="phrases">The phrases to recognize.</param>
    /// <param name="executeCommand">
    /// The command to execute when any of the phrases is recognized.
    /// </param>
    public void AddCommand(string ruleName, string[] phrases,
        Action<RecognitionResult> executeCommand)
    {
        var builder = new GrammarBuilder2()
            .OneOf(phrases);

        AddCommand(ruleName, builder, executeCommand);
    }
}