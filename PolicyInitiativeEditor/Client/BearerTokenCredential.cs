using Azure.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace PolicyInitiativeEditor.Client
{
    public class BearerTokenCredential : TokenCredential
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly NavigationManager _navigation;
        private readonly IEnumerable<string> _scopes;

        public BearerTokenCredential(IAccessTokenProvider tokenProvider, NavigationManager navigation, IEnumerable<string> scopes)
        {
            _tokenProvider = tokenProvider;
            _navigation = navigation;
            _scopes = scopes;
        }

        public override Azure.Core.AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var tokenResult = _tokenProvider.RequestAccessToken()
                .AsTask()
                .GetAwaiter()
                .GetResult();

            if (tokenResult.TryGetToken(out var token))
            {
                return new Azure.Core.AccessToken(token.Value, token.Expires);
            }
            else
            {
                throw new AccessTokenNotAvailableException(_navigation, tokenResult, _scopes);
            }
        }

        public override async ValueTask<Azure.Core.AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var tokenResult = await _tokenProvider.RequestAccessToken()
                .ConfigureAwait(false);

            if (tokenResult.TryGetToken(out var token))
            {
                return new Azure.Core.AccessToken(token.Value, token.Expires);
            }
            else
            {
                throw new AccessTokenNotAvailableException(_navigation, tokenResult, _scopes);
            }
        }
    }
}
