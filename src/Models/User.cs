namespace Models;

/// <summary>
/// Пользователь.
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор роли.
    /// </summary>
    public Guid RoleId { get; set; }
    
    /// <summary>
    /// Роль.
    /// </summary>
    public virtual Role Role { get; set; } = null!;
    
    /// <summary>
    /// Имя.
    /// </summary>
    public string Name { get; set; } = string.Empty;
	
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string Surname { get; set; } = string.Empty;
	
    /// <summary>
    /// Отчество.
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Полное имя.
    /// </summary>
    public string FullName => $"{Surname} {Name} {(string.IsNullOrEmpty(Patronymic) ? string.Empty : Patronymic[0] + '.')}".Trim();
    
    /// <summary>
    /// Телефон.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Электронная почта.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Примечание.
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// Токен обновления.
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Дата истечения токена обновления.
    /// </summary>
    public DateTime? RefreshTokenExpires { get; set; }
    
    /// <summary>
    /// Признак блокировки.
    /// </summary>
    public bool IsBlocked { get; set; }
    
    /// <summary>
    /// Причина блокировки.
    /// </summary>
    public string? BlockReason { get; set; }
    
    /// <summary>
    /// Дата создания.
    /// </summary>
    public DateTime Created { get; init; } = DateTime.UtcNow;
}