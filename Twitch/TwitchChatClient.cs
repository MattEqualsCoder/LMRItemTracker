using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Models;

namespace LMRItemTracker.Twitch
{
    public class TwitchChatClient : IChatClient
    {
        private readonly TwitchClient _twitchClient;
        private readonly IChatApi _chatApi;

        public TwitchChatClient(ILogger<TwitchChatClient> logger, ILoggerFactory loggerFactory, IChatApi chatApi)
        {
            Logger = logger;
            _twitchClient = new(logger: loggerFactory.CreateLogger<TwitchClient>());
            _twitchClient.OnConnected += _twitchClient_OnConnected;
            _twitchClient.OnDisconnected += _twitchClient_OnDisconnected;
            _twitchClient.OnMessageReceived += _twitchClient_OnMessageReceived;
            _chatApi = chatApi;
        }

        public event EventHandler? Connected;

        public event EventHandler? Disconnected;

        public event EventHandler? SendMessageFailure;

        public event MessageReceivedEventHandler? MessageReceived;

        public string? ConnectedAs { get; protected set; }

        public string? Channel { get; protected set; }

        public string? OAuthToken { get; protected set; }

        public string? Id { get; protected set; }

        public bool IsConnected { get; protected set; }

        protected ILogger<TwitchChatClient> Logger { get; }

        public void Connect(string userName, string oauthToken, string channel, string id)
        {
            if (!_twitchClient.IsInitialized)
            {
                var credentials = new ConnectionCredentials(userName, oauthToken);
                _twitchClient.Initialize(credentials, channel);
            }

            _twitchClient.Connect();
            ConnectedAs = userName;
            Channel = channel;
            if (string.IsNullOrEmpty(_chatApi.GetUserIdOverride()))
            {
                Id = userName.Equals(channel, StringComparison.OrdinalIgnoreCase) ? id : null;
            }
            else
            {
                Id = _chatApi.GetUserIdOverride();
            }
            
            _chatApi.SetAccessToken(oauthToken);
        }

        public void Disconnect()
        {
            if (_twitchClient.IsInitialized)
            {
                IsConnected = false;
                _twitchClient.Disconnect();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task SendMessageAsync(string message, bool announce = false)
        {
            try
            {
                _twitchClient.SendMessage(Channel, message);
            }
            catch (BadStateException e)
            {
                Logger.LogError(e, "Error in sending chat message");
                SendMessageFailure?.Invoke(this, new());
            }

            return Task.CompletedTask;
        }

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, new());
        }

        protected virtual void OnDisconnected()
        {
            Logger.LogWarning("Connection to chat lost");
            Disconnected?.Invoke(this, new());
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }

        private void _twitchClient_OnConnected(object? sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            IsConnected = true;
            OnConnected();
        }

        private void _twitchClient_OnDisconnected(object? sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            if (IsConnected)
            {
                OnDisconnected();
            }
            IsConnected = false;
        }

        private void _twitchClient_OnMessageReceived(object? sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            OnMessageReceived(new MessageReceivedEventArgs(new TwitchChatMessage(e.ChatMessage)));
        }

        public async Task<string?> CreatePollAsync(string title, ICollection<string> options, int duration)
        {
            // Create the poll object
            var poll = new TwitchPoll()
            {
                BroadcasterId = Id,
                Title = title,
                Choices = options.Select(x => new TwitchPollChoice()
                {
                    Title = x
                }).ToList(),
                Duration = duration
            };

            var pollApiResponse = await _chatApi.MakeApiCallAsync<TwitchPoll, TwitchPoll>("polls", poll, HttpMethod.Post, default);

            return pollApiResponse != null && pollApiResponse.IsSuccessful ? pollApiResponse.Id : null;
        }

        public async Task<ChatPoll> CheckPollAsync(string id)
        {
            var pollApiResponse = await _chatApi.MakeApiCallAsync<TwitchPoll>($"polls?broadcaster_id={Id}&id={id}", HttpMethod.Get, default);

            if (pollApiResponse == null)
            {
                return new ChatPoll
                {
                    IsPollComplete = true,
                    IsPollSuccessful = false
                };
            }

            Logger.LogInformation("Poll complete with status {0} and winning choice of {1}", pollApiResponse.Status, pollApiResponse.WinningChoice?.Title);

            return new()
            {
                IsPollComplete = pollApiResponse.IsPollComplete,
                IsPollSuccessful = pollApiResponse.IsPollSuccessful,
                WinningChoice = pollApiResponse.WinningChoice?.Title
            };
        }

        public async Task<ChatPrediction?> CreatePredictionAsync(string title, ICollection<string> options, int duration)
        {
            // Create the prediction object
            var prediction = new TwitchPrediction()
            {
                BroadcasterId = Id,
                Title = title,
                Outcomes = options.Select(x => new TwitchPredictionOutcome()
                {
                    Title = x
                }).ToList(),
                PredictionWindow = duration
            };

            var pollApiResponse = await _chatApi.MakeApiCallAsync<TwitchPrediction, TwitchPrediction>("predictions", prediction, HttpMethod.Post, default);

            return new ChatPrediction()
            {
                Id = pollApiResponse?.Id,
                IsPredictionSuccessful = pollApiResponse?.IsSuccessful == true && !string.IsNullOrEmpty(pollApiResponse?.Id),
                OutcomeIds = pollApiResponse?.IsSuccessful == true ? pollApiResponse.Outcomes.ToDictionary(x => x.Title, x => x.Id) : new Dictionary<string, string>()
            };
        }

        public async Task<ChatPrediction> CheckPredictionAsync(ChatPrediction prediction)
        {
            var pollApiResponse = await _chatApi.MakeApiCallAsync<TwitchPrediction>($"predictions?broadcaster_id={Id}&id={prediction.Id}", HttpMethod.Get, default);

            if (pollApiResponse == null)
            {
                return new ChatPrediction
                {
                    Id = prediction.Id,
                    IsPredictionComplete = true,
                    IsPredictionSuccessful = false,
                    OutcomeIds = prediction.OutcomeIds
                };
            }

            Logger.LogInformation("Prediction GET call complete with status {0}", pollApiResponse.Status);

            return ParsePredictionResults(pollApiResponse);
        }

        public async Task<ChatPrediction> EndPredictionAsync(ChatPrediction prediction, string? winningOutcomeId = null, bool cancelled = false)
        {
            var status = !string.IsNullOrEmpty(winningOutcomeId) ? "RESOLVED" : (cancelled ? "CANCELED" : "LOCKED");

            var request = new TwitchPrediction()
            {
                BroadcasterId = Id,
                Id = prediction.Id,
                Status = status,
                WinningOutcomeId = winningOutcomeId
            };

            var pollApiResponse = await _chatApi.MakeApiCallAsync<TwitchPrediction, TwitchPrediction>("predictions", request, new HttpMethod("PATCH"), default);

            if (pollApiResponse == null)
            {
                return new ChatPrediction
                {
                    Id = prediction.Id,
                    IsPredictionComplete = true,
                    IsPredictionSuccessful = false,
                    OutcomeIds = prediction.OutcomeIds
                };
            }

            Logger.LogInformation("Prediction PATCH call complete with status {0}", pollApiResponse.Status);

            return ParsePredictionResults(pollApiResponse);
        }

        private ChatPrediction ParsePredictionResults(TwitchPrediction pollApiResponse)
        {
            if (!pollApiResponse.IsPredictionSuccessful)
            {
                return new()
                {
                    Id = pollApiResponse.Id,
                    IsPredictionComplete = pollApiResponse.IsPredictionComplete,
                    IsPredictionSuccessful = pollApiResponse.IsPredictionSuccessful,
                    OutcomeIds = pollApiResponse.Outcomes.ToDictionary(x => x.Title, x => x.Id),
                };
            }
            else
            {
                var winningId = pollApiResponse.WinningOutcomeId;
                var winningResults = pollApiResponse.Outcomes.First(x => x.Id == winningId);
                var losingResults = pollApiResponse.Outcomes.First(x => x.Id != winningId);

                var winnerNames = new List<string>();
                var loserNames = new List<string>();
                
                try
                {
                    if (winningResults.TopPredictors != null)
                    {
                        winnerNames = winningResults.TopPredictors.Select(x => x.DisplayName)
                            .Where(x => !string.IsNullOrEmpty(x)).Cast<string>().ToList();
                    }
                    
                    if (losingResults.TopPredictors != null)
                    {
                        loserNames = losingResults.TopPredictors.Select(x => x.DisplayName)
                            .Where(x => !string.IsNullOrEmpty(x)).Cast<string>().ToList();
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Unable to parse winner/loser names");
                }
                
                return new ChatPrediction
                {
                    Id = pollApiResponse.Id!,
                    IsPredictionComplete = true,
                    IsPredictionSuccessful = true,
                    OutcomeIds = pollApiResponse.Outcomes.ToDictionary(x => x.Title, x => x.Id),
                    WinningChoice = winningResults.Title,
                    WinningCount = winningResults.UserCount,
                    PointsWon = winningResults.ChannelPointsSpent,
                    Winners = winnerNames,
                    LosingCount = losingResults.UserCount,
                    PointsLost = losingResults.ChannelPointsSpent,
                    Losers = loserNames
                };
            }
        }
    }
}
