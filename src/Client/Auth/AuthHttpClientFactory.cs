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
    /// Имя базового http-клиента.
    /// </summary>
    public static string BaseHttpClientName => "BaseHttpClient";
    
    /// <summary>
    /// Имя базового http-клиента с авторизацией.
    /// </summary>
    public static string AuthorizedBaseHttpClientName => "AuthorizedBaseHttpClient";

    /// <summary>
    /// Имена http-клиентов.
    /// </summary>
    public static List<string> HttpClientNames => [BaseHttpClientName, AuthorizedBaseHttpClientName];
    
    /// <summary>
    /// Исключенные имена http-клиентов.
    /// </summary>
    public static List<string> ExcludedHttpClientNames => [BaseHttpClientName, AuthorizedBaseHttpClientName];
    
    /// <summary>
    /// Создать базовый http-клиент.
    /// </summary>
    public HttpClient CreateClient(string name) => _httpClientFactory.CreateClient(name);
    
    /// <summary>
    /// Создать http-клиент с авторизацией.
    /// </summary>
    /// <returns>Http-клиент с авторизацией.</returns>
    public HttpClient CreateBaseClient() => CreateClient(BaseHttpClientName);
    
    /// <summary>
    /// Создать базовый http-клиент с авторизацией.
    /// </summary>
    public HttpClient CreateAuthorizedBaseClient() => CreateClient(AuthorizedBaseHttpClientName);
}
