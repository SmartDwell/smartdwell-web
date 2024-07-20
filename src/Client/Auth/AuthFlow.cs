using System.Net.Http.Json;
using Contracts.Auth;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Client.Auth;

/// <summary>
/// Поток авторизации.
/// </summary>
public class AuthFlow
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateProvider _authStateProvider;
    private readonly NavigationManager _navigation;

    /// <summary>
    /// Конструктор класса <see cref="AuthFlow"/>.
    /// </summary>
    /// <param name="httpClient">Http-клиент.</param>
    /// <param name="authStateProvider">Поставщик состояния авторизации.</param>
    /// <param name="navigation">Менеджер навигации.</param>
    public AuthFlow(HttpClient httpClient, AuthStateProvider authStateProvider, NavigationManager navigation)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _navigation = navigation;
    }
    
    /// <summary>
    /// Аутентификация пользователя.
    /// </summary>
    /// <param name="customReturnUrl">Пользовательский адрес возврата.</param>
    public void Login(string? customReturnUrl = null)
    {
        var loginUrl = _navigation.ToAbsoluteUri($"{_httpClient.BaseAddress}login/{GetEncodedReturnUrl(_navigation, customReturnUrl)}");
        _navigation.NavigateTo(loginUrl.ToString(), true, true);
    }

    /// <summary>
    /// Выход из системы.
    /// </summary>
    /// <param name="customReturnUrl">Пользовательский адрес возврата.</param>
    public void Logout(string? customReturnUrl = null)
    {
        var logoutUrl = _navigation.ToAbsoluteUri($"{_httpClient.BaseAddress}logout/{GetEncodedReturnUrl(_navigation, customReturnUrl)}");
        _navigation.NavigateTo(logoutUrl.ToString(), true, true);
    }

    /// <summary>
    /// Перенаправление на страницу 403.
    /// </summary>
    public void Forbidden()
    {
        var forbiddenUrl = _navigation.ToAbsoluteUri($"{_httpClient.BaseAddress}403");
        _navigation.NavigateTo(forbiddenUrl.ToString(), true, true);
    }
    
    /// <summary>
    /// Обновление токенов.
    /// </summary>
    /// <returns>True, если токены обновлены, иначе false.</returns>
    /// <exception cref="Exception">Ошибка при обновлении токенов.</exception>
    public async Task RefreshTokenAsync(RefreshTokensDto refreshTokensDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", refreshTokensDto);

        if (response.IsSuccessStatusCode)
        {
            var tokens = await response.Content.ReadFromJsonAsync<TokensDto>()
                         ?? throw new Exception("Ошибка при обновлении токенов.");
            await _authStateProvider.LoginAsync(tokens.AccessToken, tokens.RefreshToken);
            return;
        }

        await _authStateProvider.LogoutAsync();
    }
    
    private static string GetEncodedReturnUrl(NavigationManager navigation, string? customReturnUrl)
    {
        var returnUrl = customReturnUrl != null ? navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
        return Uri.EscapeDataString(returnUrl ?? navigation.Uri);
    }
}
