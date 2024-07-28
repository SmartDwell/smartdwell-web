namespace Shared;

/// <summary>
/// Параметры конфигурации.
/// </summary>
public class ConfigurationOptions
{
    /// <summary>
    /// Путь к серверу работы с заявками.
    /// </summary>
    public string RequestsServiceUrl { get; set; } = string.Empty;
}
