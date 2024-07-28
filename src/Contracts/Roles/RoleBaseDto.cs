namespace Contracts.Roles;

/// <summary>
/// Базовая модель роли.
/// </summary>
public class RoleBaseDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
