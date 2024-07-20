namespace Contracts.Auth;

/// <summary>
/// Модель данных для старта авторизации.
/// </summary>
public class AuthRequestCodeDto
{
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login { get; set; } = string.Empty;
    
    /// <summary>
    /// Тип логина при авторизации.
    /// </summary>
    public AuthLoginType AuthLoginType { get; set; }
    
    /// <summary>
    /// Информация об устройстве.
    /// </summary>
    public string? DeviceDescription { get; set; } = string.Empty;
}

/// <summary>
/// Тип логина при авторизации.
/// </summary>
public enum AuthLoginType
{
    /// <summary>
    /// Электронная почта.
    /// </summary>
    Email = 0,
}