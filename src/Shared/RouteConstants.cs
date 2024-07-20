namespace Shared;

/// <summary>
/// Константы маршрутов.
/// </summary>
public static class RouteConstants
{
    /// <summary>
    /// Данные настроек.
    /// </summary>
    public static class ConfigurationData
    {
        /// <summary>
        /// Базовый маршрут.
        /// </summary>
        public const string Route = "/api/configuration";

        /// <summary>
        /// Получение настроек.
        /// </summary>
        public const string Options = "/options";
        
        /// <summary>
        /// Получение маршрута настроек.
        /// </summary>
        /// <returns>Маршрут настроек.</returns>
        public static string GetOptionsUrl() => $"{Route}{Options}";
    }
    
    /// <summary>
    /// Данные аутентификации
    /// </summary>
    public static class AuthData
    {
        /// <summary>
        /// Базовый маршрут
        /// </summary>
        public const string Route = "/api/auth";
        
        /// <summary>
        /// Начало аутентификации
        /// </summary>
        public const string Start = "/start";
        
        /// <summary>
        /// Завершение аутентификации
        /// </summary>
        public const string Complete = "/complete";
        
        /// <summary>
        /// Обновление токенов
        /// </summary>
        public const string Refresh = "/refresh";
    }
}
