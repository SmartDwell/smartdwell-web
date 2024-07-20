using System.Net;
using System.Net.Http.Headers;
using Contracts.Auth;

namespace Client.Auth;

/// <summary>
/// Обработчик http-клиента с авторизацией.
/// </summary>
public class AuthHttpClientHandler : DelegatingHandler
{
    private readonly AuthFlow _authFlow;
    private readonly AuthStateProvider _authStateProvider;

    /// <summary>
    /// Конструктор класса <see cref="AuthHttpClientHandler"/>.
    /// </summary>
    /// <param name="authFlow">Поток авторизации.</param>
    /// <param name="authStateProvider">Поставщик состояния авторизации.</param>
    public AuthHttpClientHandler(AuthFlow authFlow, AuthStateProvider authStateProvider)
    {
        _authFlow = authFlow;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Отправить запрос.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = _authStateProvider.AccessToken;
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshTokensDto = new RefreshTokensDto
            {
                RefreshToken = _authStateProvider.RefreshToken
            };

            await _authFlow.RefreshTokenAsync(refreshTokensDto);
            
            if (_authStateProvider.IsAuthenticated)
            {
                accessToken =  _authStateProvider.AccessToken;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _authStateProvider.LogoutAsync();
                _authFlow.Login();
            }
        }

        return response;
    }
}
