using System.Net.Http.Json;
using Contracts.Auth;
using Shared;

namespace Client.Auth;

/// <summary>
/// Сервис авторизации.
/// </summary>
public class AuthService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateProvider _authStateProvider;
    private readonly AuthFlow _authFlow;

    /// <summary>
    /// Конструктор класса <see cref="AuthService"/>.
    /// </summary>
    /// <param name="httpClient">Http-клиент.</param>
    /// <param name="authStateProvider">Поставщик состояния авторизации.</param>
    /// <param name="authFlow">Поток авторизации.</param>
    public AuthService(HttpClient httpClient, AuthStateProvider authStateProvider, AuthFlow authFlow)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _authFlow = authFlow;
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

            await _authStateProvider.LoginAsync(tokensDto.AccessToken, tokensDto.RefreshToken);
            return true;
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new Exception($"Ошибка: {errorMessage}.");
    }

    /// <summary>
    /// Выход из системы.
    /// </summary>
    public async Task LogoutAsync()
    {
        await _authStateProvider.LogoutAsync();
        _authFlow.Logout();
    }
    
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
