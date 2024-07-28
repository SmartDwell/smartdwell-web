namespace Contracts.Roles;

/// <summary>
/// Модель добавления роли.
/// </summary>
public class RoleAddDto
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Политики.
    /// </summary>
    public List<string> Policies { get; set; } = [];
}
