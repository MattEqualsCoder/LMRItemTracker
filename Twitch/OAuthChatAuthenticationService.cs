using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public abstract class OAuthChatAuthenticationService : IChatAuthenticationService
    {
        public abstract Uri GetOAuthUrl(Uri redirectUri);

        private string? _accessToken = null;
        private string _localUrl = "http://localhost:42069";
        private HttpListener? _listener;

        public virtual async Task<string?> GetTokenInteractivelyAsync(CancellationToken cancellationToken)
        {
            var stoppingToken = new CancellationTokenSource();
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken.Token, cancellationToken);

            try
            {

                // Create a Http server and start listening for incoming connections
                _listener = new HttpListener();
                _listener.Prefixes.Add(_localUrl + "/");
                _listener.Start();
                Receive();

                var authUrl = GetOAuthUrl(new Uri(_localUrl));
                Process.Start(new ProcessStartInfo
                {
                    FileName = authUrl.ToString(),
                    UseShellExecute = true
                });

                while (_accessToken == null)
                {
                    await Task.Delay(1000);
                }

                await Task.Delay(1000);

                _listener.Close();
            }
            catch
            {
                // do nothing
            }

            return _accessToken;
        }

        public abstract Task<AuthenticatedUserData?> GetAuthenticatedUserDataAsync(string accessToken, CancellationToken cancellationToken);
        public abstract Task<bool> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken);
        public abstract Task<bool> RevokeTokenAsync(string accessToken, CancellationToken cancellationToken);

        private void Receive()
        {
            _listener?.BeginGetContext(ListenerCallback, _listener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (_listener?.IsListening == true)
            {
                var context = _listener.EndGetContext(result);
                var request = context.Request;
                var responseText = "You can now close this browser";
                if (request.QueryString.Count == 0)
                {
                    responseText = @"
<script>
    fetch('?forwarded=true&' + document.location.hash.slice(1), { method: 'POST' })
        .then(response => {
            document.write(response.ok ? 'You may now close this window.' : 'Oops. Something went wrong.');
            window.close();
        });
</script>";
                }
                else
                {
                    try
                    {
                        _accessToken = request.QueryString["access_token"];
                        _accessToken ??= "";
                    }
                    catch
                    {
                        _accessToken = "";
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(responseText);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.WriteAsync(data, 0, data.Length);
                context.Response.Close();
                Receive();
            }
        }
    }

}
