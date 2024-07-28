using System.ComponentModel.DataAnnotations;
using Contracts.Roles;

namespace Contracts.Users;

/// <summary>
/// Модель пользователя.
/// </summary>
public class UserDto
{
	/// <summary>
	/// Идентификатор.
	/// </summary>
	[Required(ErrorMessage = "Не указан идентификатор.")]
	public Guid Id { get; set; }
    
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
	
	/// <summary>
	/// Полное имя.
	/// </summary>
	public string FullName => $"{Surname} {Name} {Patronymic}".Trim();
	
	/// <summary>
	/// Строка для поиска.
	/// </summary>
	public string SearchString => $"{FullName} {Phone} {Email} {Role.Name}".ToLower();
	
	/// <summary>
	/// Маскированный телефон вида +7 (999) 999-99-99.
	/// </summary>
	public string MaskedPhone => Phone.Length == 11
		? $"+7 ({Phone[1..4]}) {Phone[4..7]}-{Phone[7..9]}-{Phone[9..11]}"
		: Phone;
	
	/// <summary>
	/// Клонировать.
	/// </summary>
	/// <returns>Клон.</returns>
	public UserDto Clone() => new()
	{
		Id = Id,
		Phone = Phone,
		Email = Email,
		Name = Name,
		Surname = Surname,
		Patronymic = Patronymic,
		Role = new RoleBaseDto
		{
			Id = Role.Id,
			Name = Role.Name
		}
	};
}
