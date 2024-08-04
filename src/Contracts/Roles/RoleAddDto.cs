using System.ComponentModel.DataAnnotations;

namespace Contracts.Roles;

/// <summary>
/// Модель добавления роли.
/// </summary>
public class RoleAddDto
{
    /// <summary>
    /// Название.
    /// </summary>
    [Required(ErrorMessage = "Не указано название.")]
    [MinLength(2, ErrorMessage = "Название должно содержать не менее 2 символов.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Политики.
    /// </summary>
    public List<string> Policies { get; set; } = [];
}
