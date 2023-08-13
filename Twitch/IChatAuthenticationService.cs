using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public interface IChatAuthenticationService
    {
        Task<string?> GetTokenInteractivelyAsync(CancellationToken cancellationToken);

        Task<AuthenticatedUserData?> GetAuthenticatedUserDataAsync(string accessToken, CancellationToken cancellationToken);
        Task<bool> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken);
        Task<bool> RevokeTokenAsync(string accessToken, CancellationToken cancellationToken);
    }
}
