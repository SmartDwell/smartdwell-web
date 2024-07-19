namespace Models;

/// <summary>
/// Роль.
/// </summary>
public class Role
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Политики.
    /// </summary>
    public List<string> Policies { get; set; } = new();

    /// <summary>
    /// Пользователи с этой ролью.
    /// </summary>
    public virtual List<User> Users { get; set; } = new();
}