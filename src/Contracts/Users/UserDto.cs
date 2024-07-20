namespace Contracts.Users;

/// <summary>
/// Модель пользователя.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Телефон.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Электронная почта.
    /// </summary>
    public string Email { get; set; } = string.Empty;
	
    /// <summary>
    /// Имя.
    /// </summary>
    public string Name { get; set; } = string.Empty;
	
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string Surname { get; set; } = string.Empty;
    
    /// <summary>
    /// Роль.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}