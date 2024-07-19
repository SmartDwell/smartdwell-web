using Seljmov.AspNet.Commons.Options;
using Seljmov.Blazor.Identity.Shared;

namespace Server.Options;

/// <summary>
/// Настройки приложения.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// Настройки шаблона сообщений.
    /// </summary>
    public CodeTemplateOptions CodeTemplateOptions { get; set; } = default!;
    
    /// <summary>
    /// Настройки SMTP-клиента.
    /// </summary>
    public SmtpClientOptions SmtpClientOptions { get; set; } = default!;
    
    /// <summary>
    /// Настройки JWT-токена.
    /// </summary>
    public JwtOptions JwtOptions { get; set; } = default!;
}

/// <summary>
/// Настройки сообщений для авторизации.
/// </summary>
/// <param name="From">От кого.</param>
/// <param name="Subject">Тема письма.</param>
/// <param name="Body">Содержание письма.</param>
public record CodeTemplateOptions(string From, string Subject, string Body);

/// <summary>
/// Настройки SMTP-сервера.
/// </summary>
/// <param name="Host">Хост.</param>
/// <param name="Port">Порт.</param>
/// <param name="Email">Электронная почта.</param>
/// <param name="Password">Пароль.</param>
/// <param name="EnableSsl">Включение SSL.</param>
public record SmtpClientOptions(string Host, int Port, string Email, string Password, bool EnableSsl);