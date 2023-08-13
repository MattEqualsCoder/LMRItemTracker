using LMRItemTracker.Configs;
using LMRItemTracker.Twitch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMRItemTracker.VoiceTracker
{
    public class ChatModule
    {
        private ILogger<ChatModule> _logger;
        private TextToSpeechService _ttsService;
        private IChatClient _chatClient;
        private TwitchConfig _config;
        private TrackerConfig _trackerConfig;
        private string _localUsername;
        private readonly Dictionary<string, int> _usersGreetedTimes = new();
        private ChatPrediction? _currentPrediction;
        private TwitchPredictionConfig? _currentPredictionConfig;
        private string? _currentPredictionGoodId;
        private string? _currentPredictionBadId;
        private readonly Random _random = new();

        public ChatModule(ILogger<ChatModule> logger, TextToSpeechService ttsService, VoiceRecognitionService voiceService, ConfigService configService, IChatClient chatClient, ChoiceService choices)
        {
            _logger = logger;
            _ttsService = ttsService;
            _chatClient = chatClient;
            _chatClient.Connected += ChatClient_Connected;
            _chatClient.MessageReceived += ChatClient_MessageReceived;
            _config = configService.Config.TwitchConfig;
            _trackerConfig = configService.Config;
            
            voiceService.AddCommand("start prediction",
                new GrammarBuilder()
                    .Append("Hey tracker, ")
                    .Append(choices.PredictionKey, choices.GetStartPredictionNames()),
                result =>
                {
                    var predictionConfig = choices.GetPredictionConfigFromResult(result);
                    _ = StartPredictionPoll(predictionConfig);
                }
            );
            
            voiceService.AddCommand("resolve prediction good",
                new GrammarBuilder()
                    .Append("Hey tracker, ")
                    .Append(choices.PredictionKey, choices.GetResolveGoodPredictionNames()),
                result =>
                {
                    var predictionConfig = choices.GetPredictionConfigFromResult(result);
                    _ = ResolvePredictionPoll(predictionConfig, true);
                }
            );
            
            voiceService.AddCommand("resolve prediction bad",
                new GrammarBuilder()
                    .Append("Hey tracker, ")
                    .Append(choices.PredictionKey, choices.GetResolveBadPredictionNames()),
                result =>
                {
                    var predictionConfig = choices.GetPredictionConfigFromResult(result);
                    _ = ResolvePredictionPoll(predictionConfig, false);
                }
            );
        }

        public async Task StartPredictionPoll(TwitchPredictionConfig config)
        {
            var title = config.PredictionTitles.ToString();
            var options = config.PredictionOptions[_random.Next(0, config.PredictionOptions.Count)];

            if (string.IsNullOrEmpty(title) || options == null)
            {
                _logger.LogError("Prediction title or options are missing");
                _ttsService.Say(_trackerConfig.Responses.Error);
                return;
            }

            _currentPredictionConfig = config;
            _currentPrediction = await _chatClient.CreatePredictionAsync(title!, new List<string>() { options.Good, options.Bad }, 120);
            if (_currentPrediction?.IsPredictionSuccessful != true)
            {
                _logger.LogError("Error creating prediction poll");
                _ttsService.Say(_trackerConfig.Responses.Error);
                return;
            }

            if (!_currentPrediction.OutcomeIds.TryGetValue(options.Good, out _currentPredictionGoodId) ||
                !_currentPrediction.OutcomeIds.TryGetValue(options.Bad, out _currentPredictionBadId))
            {
                _logger.LogError("Created prediction poll but couldn't map outcomes");
                _ttsService.Say(_trackerConfig.Responses.Error);
                return;
            }
            
            _ttsService.Say(config.StartResponses);
        }

        public async Task ResolvePredictionPoll(TwitchPredictionConfig config, bool isGoodResult)
        {
            if (_currentPrediction == null || _currentPredictionConfig == null)
            {
                _logger.LogWarning("No current active prediction");
                _ttsService.Say(_trackerConfig.Responses.Error);
                return;
            }
            var winnerId = isGoodResult ? _currentPredictionGoodId : _currentPredictionBadId;

            try
            {
                var predictionResults = await _chatClient.EndPredictionAsync(_currentPrediction, winnerId);
                
                if (predictionResults.IsPredictionSuccessful != true)
                {
                    _currentPrediction = null;
                    _currentPredictionConfig = null;
                    _logger.LogError("Error resolving prediction poll");
                    _ttsService.Say(_trackerConfig.Responses.Error);
                    return;
                }
            
                _ttsService.Say(_currentPredictionConfig.ResolvedResponses, predictionResults.WinningChoice, 
                    predictionResults.WinningCount, predictionResults.PointsWon, predictionResults.Winners.FirstOrDefault() ?? "Nobody", 
                    predictionResults.LosingCount, predictionResults.PointsLost, predictionResults.Losers.FirstOrDefault() ?? "Nobody");
            
                _currentPrediction = null;
                _currentPredictionConfig = null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error resolving prediction poll");
                _currentPrediction = null;
                _currentPredictionConfig = null;
                _ttsService.Say(_trackerConfig.Responses.Error);
            }
        }

        public void Connect(string username, string authToken, string channel, string id)
        {
            if (_chatClient.IsConnected)
            {
                _chatClient.Disconnect();
            }
            _localUsername = username;
            _chatClient.Connect(username, authToken, channel, id);
        }

        private void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var senderName = CorrectUserNamePronunciation(e.Message.Sender);

                if (RespondToChat)
                {
                    TryRespondToGreetings(e.Message, senderName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An unexpected error occurred while processing incoming chat messages.");
            }
        }

        private string CorrectUserNamePronunciation(string username)
        {
            var correctedUserName = _config.UserNamePronunciations
                .SingleOrDefault(x => x.Key.Equals(username, StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrEmpty(correctedUserName.Value) ? username.Replace('_', ' ') : correctedUserName.Value;
        }

        private void TryRespondToGreetings(ChatMessage message, string senderNamePronunciation)
        {
            foreach (var recognizedGreeting in _config.RecognizedGreetings)
            {
                if (Regex.IsMatch(message.Text, recognizedGreeting, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                {
                    // Sass if it was the broadcaster
                    if (message.SenderUserName.Equals(_localUsername, StringComparison.OrdinalIgnoreCase)
                        && _config.GreetedBySelf != null)
                    {
                        _ttsService.Say(_config.GreetedBySelf, senderNamePronunciation);
                        break;
                    }

                    // Otherwise, keep track of the number of times someone said hi
                    if (_usersGreetedTimes.TryGetValue(message.Sender, out var greeted))
                    {
                        if (greeted >= 2)
                            break;

                        _ttsService.Say(_config.GreetedTwice, senderNamePronunciation);
                        _usersGreetedTimes[message.Sender]++;
                    }
                    else
                    {
                        _ttsService.Say(_config.GreetingResponses, senderNamePronunciation);
                        _usersGreetedTimes.Add(message.Sender, 1);
                    }
                    break;
                }
            }
        }

        private async void ChatClient_Connected(object sender, EventArgs e)
        {
            _ttsService.Say(_config.WhenConnected);
        }

        public bool RespondToChat { get; set; }
        public bool OpenPolls { get; set; }
        
        
    }
}
