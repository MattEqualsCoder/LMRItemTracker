using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public interface IChatClient : IDisposable
    {
        event EventHandler? Connected;

        event EventHandler? Disconnected;

        event EventHandler? SendMessageFailure;

        event MessageReceivedEventHandler? MessageReceived;

        bool IsConnected { get; }
        string? ConnectedAs { get; }
        string? Channel { get; }

        void Connect(string userName, string oauthToken, string channel, string id);

        void Disconnect();

        Task SendMessageAsync(string message, bool announce = false);

        Task<string?> CreatePollAsync(string title, ICollection<string> options, int duration);

        Task<ChatPoll> CheckPollAsync(string id);

        Task<ChatPrediction?> CreatePredictionAsync(string title, ICollection<string> options, int duration);

        Task<ChatPrediction> CheckPredictionAsync(ChatPrediction prediction);

        Task<ChatPrediction> EndPredictionAsync(ChatPrediction prediction, string? winningOutcomeId, bool cancelled = false);
    }
}
