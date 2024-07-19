using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Client.Shared;

/// <summary>
/// Сервис авторизации.
/// </summary>
public class AuthService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly TokenRepository _tokenRepository;
    private readonly NavigationManager _navigation;

    /// <summary>
    /// Конструктор класса <see cref="AuthService"/>.
    /// </summary>
    /// <param name="httpClient">Http-клиент.</param>
    /// <param name="tokenRepository">Репозиторий токенов.</param>
    /// <param name="navigation">Менеджер навигации.</param>
    public AuthService(HttpClient httpClient, TokenRepository tokenRepository, NavigationManager navigation)
    {
        _httpClient = httpClient;
        _tokenRepository = tokenRepository;
        _navigation = navigation;
    }

    /// <summary>
    /// Запрос кода.
    /// </summary>
    /// <param name="requestCodeDto">Данные запроса кода.</param>
    /// <returns>Тикет, который необходимо использовать для подтверждения кода.</returns>
    /// <exception cref="Exception">Ошибка при получении кода.</exception>
    public async Task<TicketDto> RequestCodeAsync(AuthRequestCodeDto requestCodeDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/start", requestCodeDto);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TicketDto>()
                   ?? throw new Exception("Ошибка при получении кода.");
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new Exception($"Ошибка: {errorMessage}");
    }

    /// <summary>
    /// Подтверждение кода.
    /// </summary>
    /// <param name="verifyCodeDto">Данные подтверждения кода.</param>
    /// <returns>Результат подтверждения кода.</returns>
    /// <exception cref="Exception">Ошибка при отправке кода.</exception>
    public async Task<bool> VerifyCodeAsync(AuthVerifyCodeDto verifyCodeDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/complete", verifyCodeDto);
        if (response.IsSuccessStatusCode)
        {
            var tokensDto = await response.Content.ReadFromJsonAsync<TokensDto>()
                            ?? throw new Exception("Ошибка при отправке кода.");

            //await SaveTokens(tokensDto);
            return true;
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new Exception($"Ошибка: {errorMessage}.");
    }
    
    private async Task SaveTokens(TokensDto tokensDto)
    {
        var response = await _httpClient.PostAsJsonAsync(RouteConstants.ConfigurationData.SaveTokensUrl(), tokensDto);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка: {errorMessage}");
        }
        
        await _tokenRepository.SetTokensAsync(tokensDto.AccessToken, tokensDto.RefreshToken);
    }

    /// <summary>
    /// Выход из системы.
    /// </summary>
    public async Task LogoutAsync()
    {
        await _tokenRepository.ClearTokensAsync();

        _navigation.NavigateTo("/login");
    }
    
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
