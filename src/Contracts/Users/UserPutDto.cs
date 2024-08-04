using System.ComponentModel.DataAnnotations;
using Contracts.Roles;

namespace Contracts.Users;

/// <summary>
/// Модель добавления пользователя.
/// </summary>
public class UserPutDto
{
	/// <summary>
	/// Идентификатор.
	/// </summary>
	public Guid? Id { get; set; }
	
    /// <summary>
    /// Телефон.
    /// </summary>
    [Required(ErrorMessage = "Не указан телефон.")]
    [RegularExpression(@"^7\d{10}$", ErrorMessage = "Некорректный формат телефона. Пример: +79991234567.")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Электронная почта.
    /// </summary>
    [Required(ErrorMessage = "Не указана электронная почта.")]
    [EmailAddress(ErrorMessage = "Некорректный формат электронной почты.")]
    public string Email { get; set; } = string.Empty;
	
    /// <summary>
    /// Имя.
    /// </summary>
    [Required(ErrorMessage = "Не указано имя.")]
    [MinLength(2, ErrorMessage = "Имя должно содержать не менее 2 символов.")]
    public string Name { get; set; } = string.Empty;
	
    /// <summary>
    /// Фамилия.
    /// </summary>
    [Required(ErrorMessage = "Не указана фамилия.")]
    [MinLength(2, ErrorMessage = "Фамилия должна содержать не менее 2 символов.")]
    [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Фамилия должна содержать только буквы.")]
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// Отчество.
    /// </summary>
    [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Отчество должно содержать только буквы.")]
    public string? Patronymic { get; set; }
    
    /// <summary>
    /// Роль.
    /// </summary>
    [Required(ErrorMessage = "Не указана роль.")]
    public RoleBaseDto Role { get; set; } = new();
}