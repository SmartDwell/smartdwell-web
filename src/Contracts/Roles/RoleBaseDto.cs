using System.ComponentModel.DataAnnotations;

namespace Contracts.Roles;

/// <summary>
/// Базовая модель роли.
/// </summary>
public class RoleBaseDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    [Required(ErrorMessage = "Не указан идентификатор.")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название.
    /// </summary>
    [Required(ErrorMessage = "Не указано название.")]
    [MinLength(2, ErrorMessage = "Название должно содержать не менее 2 символов.")]
    public string Name { get; set; } = string.Empty;
}
