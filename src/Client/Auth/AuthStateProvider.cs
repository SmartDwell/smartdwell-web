using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Auth;

/// <summary>
/// Поставщик состояния авторизации.
/// </summary>
public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenRepository _tokenRepository;
    
    /// <summary>
    /// Пользователь, прошедший аутентификацию.
    /// </summary>
    private User? _user;

    /// <summary>
    /// Состояние не аутентифицированного пользователя.
    /// </summary>
    private static AuthenticationState NotAuthenticatedState => new(new ClaimsPrincipal());

    /// <summary>
    /// Конструктор класса <see cref="AuthStateProvider"/>.
    /// </summary>
    /// <param name="serviceProvider">Поставщик сервисов.</param>
    public AuthStateProvider(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        _tokenRepository = scope.ServiceProvider.GetRequiredService<TokenRepository>();
    }

    /// <summary>
    /// Признак аутентифицированного пользователя 
    /// </summary>
    public bool IsAuthenticated => _user != null;
    
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public string Id => _user?.Id ?? string.Empty;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string Fullname => $"{_user?.Name} {_user?.Surname}".Trim();

    /// <summary>
    /// Инициалы пользователя.
    /// </summary>
    public string Initials => $"{_user?.Name[0]}{_user?.Surname[0]}".Trim();
    
    /// <summary>
    /// Токен доступа.
    /// </summary>
    public string AccessToken => _user?.AccessToken ?? string.Empty;
    
    /// <summary>
    /// Токен перевыпуска.
    /// </summary>
    public string RefreshToken => _user?.RefreshToken ?? string.Empty;
    
    /// <inheritdoc cref="AuthenticationStateProvider.GetAuthenticationStateAsync" />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_user == null)
        {
            await InitializeAuthenticationStateAsync();
        }
        return GetState();
    }
    
    /// <summary>
    /// Аутентификация пользователя.
    /// </summary>
    public async Task LoginAsync(string accessToken, string refreshToken)
    {
        var principal = GetClaimsPrincipalFromJwt(accessToken);
        var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var name = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        var surname = principal.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
        
        _user = new User (id, name, surname, accessToken, refreshToken, principal);
        await _tokenRepository.SetTokensAsync(accessToken, refreshToken);
        
        NotifyAuthenticationStateChanged(Task.FromResult(GetState()));
    }

    /// <summary>
    /// Выход из системы.
    /// </summary>
    public async Task LogoutAsync()
    {
        _user = null;
        await _tokenRepository.ClearTokensAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(GetState()));
    }
    
    /// <summary>
    /// Инициализация состояния аутентификации.
    /// </summary>
    public async Task InitializeAuthenticationStateAsync()
    {
        var (accessToken, refreshToken) = await _tokenRepository.GetTokens();

        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
        {
            await LoginAsync(accessToken, refreshToken);
        }
        else
        {
            _user = null;
        }
    }
    
    /// <summary>
    /// Получить состояние аутентификации.
    /// </summary>
    private AuthenticationState GetState()
    {
        return _user != null
            ? new AuthenticationState(_user.ClaimsPrincipal)
            : NotAuthenticatedState;
    }

    /// <summary>
    /// Получить данные пользователя из токена доступа.
    /// </summary>
    /// <param name="jwt">Токен доступа.</param>
    /// <returns>Данные пользователя.</returns>
    private static ClaimsPrincipal GetClaimsPrincipalFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        var claimIdentity = new ClaimsIdentity(token.Claims, "jwt");
        return new ClaimsPrincipal(new[] { claimIdentity });
    }
    
    private record User(string Id, string Name, string Surname, string AccessToken, string RefreshToken, ClaimsPrincipal ClaimsPrincipal);
}
