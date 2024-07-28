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
	/// Отчество.
	/// </summary>
	public string? Patronymic { get; set; }
    
	/// <summary>
	/// Роль.
	/// </summary>
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
