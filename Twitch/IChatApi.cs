using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public interface IChatApi
    {
        Task<T?> MakeApiCallAsync<T>(string api, HttpMethod method, CancellationToken cancellationToken) where T : TwitchAPIResponse;
        Task<TResponse?> MakeApiCallAsync<TRequest, TResponse>(string api, TRequest requestData, HttpMethod method, CancellationToken cancellationToken) where TResponse : TwitchAPIResponse;
        void SetAccessToken(string? authToken);
        string GetClientId();
        string GetUserIdOverride();
        string GetUserTokenOverride();
    }
}
