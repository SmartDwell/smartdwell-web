namespace Contracts.Roles;

/// <summary>
/// Модель роли.
/// </summary>
public class RoleDto : RoleBaseDto
{
    /// <summary>
    /// Политики.
    /// </summary>
    public List<string> Policies { get; set; } = [];
    
    /// <summary>
    /// Количество пользователей с этой ролью.
    /// </summary>
    public int UsersCount { get; set; }
}
