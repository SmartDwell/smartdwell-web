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
    /// Данные аутентификации.
    /// </summary>
    public static class AuthData
    {
        /// <summary>
        /// Базовый маршрут.
        /// </summary>
        public const string Route = "/api/auth";
        
        /// <summary>
        /// Начало аутентификации.
        /// </summary>
        public const string Start = "/start";
        
        /// <summary>
        /// Завершение аутентификации.
        /// </summary>
        public const string Complete = "/complete";
        
        /// <summary>
        /// Обновление токенов.
        /// </summary>
        public const string Refresh = "/refresh";
    }
    
    /// <summary>
    /// Данные ролей.
    /// </summary>
    public static class RolesData
    {
        /// <summary>
        /// Базовый маршрут.
        /// </summary>
        public const string Route = "/api/roles";
        
        /// <summary>
        /// Получение списка ролей.
        /// </summary>
        public const string Roles = "/";
        
        /// <summary>
        /// Редактирование роли.
        /// </summary>
        public const string Edit = Roles;
        
        /// <summary>
        /// Добавление роли.
        /// </summary>
        public const string Add = Roles;
        
        /// <summary>
        /// Получение роли по идентификатору.
        /// </summary>
        public const string RoleById = "/{id:guid}";
        
        /// <summary>
        /// Получение списка ролей.
        /// </summary>
        public const string GetRolesUrl = $"{Route}{Roles}";
    }
    
    /// <summary>
    /// Данные пользователя.
    /// </summary>
    public static class UserData
    {
        /// <summary>
        /// Базовый маршрут
        /// </summary>
        public const string Route = "/api/users";

        /// <summary>
        /// Получение списка пользователей.
        /// </summary>
        public const string Users = "";
        
        /// <summary>
        /// Редактирование пользователя.
        /// </summary>
        public const string Put = Users;

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        public const string Delete = $"{Users}/{{id:guid}}";

        /// <summary>
        /// Получение пользователя по идентификатору.
        /// </summary>
        public const string UserById = $"{Users}/{{id:guid}}";

        /// <summary>
        /// Получение списка пользователей.
        /// </summary>
        public const string GetUsersUrl = $"{Route}{Users}";
        
        /// <summary>
        /// Редактирование пользователя.
        /// </summary>
        public const string PutUserUrl = $"{Route}{Put}";
        
        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Маршрут удаления пользователя.</returns>
        public static string DeleteUserUrl(Guid id) => $"{Route}{Delete}".Replace("{id:guid}", id.ToString());
    }
}
