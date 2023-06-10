using System;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using LMRItemTracker.Configs;
using Microsoft.Extensions.Logging;

namespace LMRItemTracker.VoiceTracker;

public class VoiceRecognitionService
{
    private readonly SpeechRecognitionEngine _speechRecognitionEngine;
    private readonly ILogger<VoiceRecognitionService> _logger;
    private readonly TrackerConfig _config;
    private readonly TextToSpeechService _tts;
    private int _recognitionThreshold;
    private int _executionThreshold;
    
    /// <summary>
    /// Dll to get the number of microphones
    /// </summary>
    /// <returns></returns>
    [DllImport("winmm.dll")]
    private static extern int waveInGetNumDevs();

    public VoiceRecognitionService(ILogger<VoiceRecognitionService> logger, TextToSpeechService tts, ConfigService configService)
    {
        _logger = logger;
        _speechRecognitionEngine = new SpeechRecognitionEngine();
        _config = configService.Config;
        _tts = tts;
    }

    /// <summary>
    /// Enables voice recognition
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Disables the voice recognition
    /// </summary>
    public void Disable()
    {
        _logger.LogInformation("Stopping speech recognition");
        _speechRecognitionEngine.RecognizeAsyncStop();
    }

    /// <summary>
    /// Adds a new voice command that matches the specified phrase.
    /// </summary>
    /// <param name="ruleName">The name of the command.</param>
    /// <param name="grammarBuilder">The grammar rules for detecting a request to tracker</param>
    /// <param name="command">The command to execute when the phrase is recognized.</param>
    public void AddCommand(string ruleName, GrammarBuilder grammarBuilder, Action<RecognitionResult> command)
    {
        try
        {
            var grammar = grammarBuilder.Build(ruleName);
            grammar.SpeechRecognized += (sender, e) =>
            {
                var confidence = e.Result.Confidence * 100;
                _logger.LogInformation("Recognized \"{Text}\" with {Confidence:P2} confidence", e.Result.Text, e.Result.Confidence);
                if (confidence >= _executionThreshold)
                {
                    try
                    {
                        command(e.Result);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unable to execute command");
                        _tts.Say(_config.Responses.Error);
                    }
                }
                else if (confidence >= _recognitionThreshold)
                {
                    _tts.Say(_config.Responses.UnrecognizedLine);
                }
            };
            _speechRecognitionEngine.LoadGrammar(grammar);
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
        var builder = new GrammarBuilder()
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
        var builder = new GrammarBuilder()
            .OneOf(phrases);

        AddCommand(ruleName, builder, executeCommand);
    }

    /// <summary>
    /// Updates the confidence thresholds for when tracker will recognize and execute requests
    /// </summary>
    /// <param name="recognitionThreshold"></param>
    /// <param name="executionThreshold"></param>
    public void UpdateThresholds(int recognitionThreshold, int executionThreshold)
    {
        _recognitionThreshold = recognitionThreshold;
        _executionThreshold = executionThreshold;
    }
}