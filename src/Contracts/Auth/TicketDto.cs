namespace Contracts.Auth;

/// <summary>
/// Тикет для авторизации.
/// </summary>
public class TicketDto
{
    /// <summary>
    /// Наименование пользователя.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор тикета.
    /// </summary>
    public string TicketId { get; set; } = string.Empty;
}