namespace Client.Auth;

/// <summary>
/// Фабрика для создания http-клиентов с авторизацией.
/// </summary>
public class AuthHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    /// <summary>
    /// Конструктор класса <see cref="AuthHttpClientFactory"/>.
    /// </summary>
    /// <param name="httpClientFactory">Фабрика http-клиентов.</param>
    public AuthHttpClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    /// <summary>
    /// Имя http-клиента с авторизацией.
    /// </summary>
    public static string AuthHttpClientName => "AuthenticatedHttpClient";
    
    /// <summary>
    /// Имя http-клиента для работы с пользователями.
    /// </summary>
    public static string UsersHttpClientName => "UsersHttpClient";

    /// <summary>
    /// Имена http-клиентов.
    /// </summary>
    public static IEnumerable<string> HttpClientNames => new[] { AuthHttpClientName, UsersHttpClientName };
    
    /// <summary>
    /// Создать http-клиент с авторизацией.
    /// </summary>
    /// <returns>Http-клиент с авторизацией.</returns>
    public HttpClient CreateClient(string name) => _httpClientFactory.CreateClient(name);
    
    /// <summary>
    /// Создать http-клиент с авторизацией.
    /// </summary>
    /// <returns>Http-клиент с авторизацией.</returns>
    public HttpClient CreateAuthenticatedClient() => CreateClient(AuthHttpClientName);
    
    /// <summary>
    /// Создать http-клиент для работы с пользователями.
    /// </summary>
    /// <returns>Http-клиент для работы с пользователями.</returns>
    public HttpClient CreateUsersClient() => CreateClient(UsersHttpClientName);
}
