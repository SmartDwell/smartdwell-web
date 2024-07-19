using System.Text;
using Client.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Seljmov.AspNet.Commons.Options;
using Server.Services.CodeSender;
using Server.Services.JwtHelper;
using Shared;

namespace Server.ApiGroups;

/// <summary>
/// Группа методов работы с аутентификацией.
/// </summary>
public static class AuthGroup
{
    /// <summary>
    /// Определение маршрутов аутентификации.
    /// </summary>
    /// <param name="endpoints">Маршруты.</param>
    public static void MapAuthGroup(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(RouteConstants.AuthData.Route);
        group.MapPost(RouteConstants.AuthData.Start, Start)
            .WithName("StartAuth")
            .WithSummary("Начало аутентификации.")
            .WithOpenApi();
        group.MapPost(RouteConstants.AuthData.Complete, Complete)
            .WithName("CompleteAuth")
            .WithSummary("Завершение аутентификации.")
            .WithOpenApi();
        group.MapPost(RouteConstants.AuthData.Refresh, RefreshTokens)
            .WithName("RefreshTokens")
            .WithSummary("Обновление токенов.")
            .WithOpenApi();
    }

    private static async Task<IResult> Start(DatabaseContext context,
        [FromBody] AuthRequestCodeDto requestCodeDto,
        [FromServices] IEmailCodeSender emailCodeSender)
    {
        var codeSender = requestCodeDto.AuthLoginType switch
        {
            AuthLoginType.Email => emailCodeSender,
            _ => null,
        };

        if (codeSender is null)
            return TypedResults.StatusCode(StatusCodes.Status501NotImplemented);

        if (string.IsNullOrEmpty(requestCodeDto.Login))
            return TypedResults.Text("Не указан логин.", statusCode: StatusCodes.Status400BadRequest);

        var user = await context.Users
            .Include(c => c.Role)
            .FirstOrDefaultAsync(c => c.Phone == requestCodeDto.Login || c.Email == requestCodeDto.Login);
        if (user is null)
            return TypedResults.Text($"Пользователь с логином {requestCodeDto.Login} не найден.",
                statusCode: StatusCodes.Status404NotFound);

        if (user.IsBlocked)
        {
            var messageBuilder = new StringBuilder("Пользователь заблокирован.");
            if (!string.IsNullOrEmpty(user.BlockReason))
                messageBuilder.Append($" Причина: {user.BlockReason}.");
            return TypedResults.Text(messageBuilder.ToString(), statusCode: StatusCodes.Status403Forbidden);
        }
        
        try
        {
            var ticket = AuthTicket.Create(requestCodeDto.Login);
            ticket.DeviceDescription = requestCodeDto.DeviceDescription;
            await codeSender.Send(ticket);
            await context.AuthTickets.AddAsync(ticket);
            await context.SaveChangesAsync();
            return TypedResults.Ok(new TicketDto
            {
                Name = user.Name,
                TicketId = ticket.Id.ToString()
            });
        }
        catch (Exception _)
        {
            return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> Complete(DatabaseContext context,
        [FromBody] AuthVerifyCodeDto verifyCodeDto,
        [FromServices] IJwtHelper jwtHelper,
        [FromServices] JwtOptions jwtOptions,
        HttpContext httpContext)
    {
        if (string.IsNullOrEmpty(verifyCodeDto.TicketId) ||
            !Guid.TryParse(verifyCodeDto.TicketId, out var ticketId) ||
            string.IsNullOrEmpty(verifyCodeDto.Code))
            return TypedResults.Text($"Некорректный тикет или код.", statusCode: StatusCodes.Status400BadRequest);

        var ticket = await context.AuthTickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null)
            return TypedResults.Text($"Не найден тикет. TicketId: {ticketId}.", statusCode: StatusCodes.Status404NotFound);

        if (ticket.IsUsed)
            return TypedResults.Text($"Тикет уже использован. TicketId: {ticketId}.", statusCode: StatusCodes.Status409Conflict);
        
        if (ticket.ExpiresAt < DateTime.UtcNow)
            return TypedResults.Text($"Тикет просрочен. TicketId: {ticketId}.", statusCode: StatusCodes.Status410Gone);
        
        if (ticket.Code != verifyCodeDto.Code)
            return TypedResults.Text($"Код не совпадает. Code: {verifyCodeDto.Code}.", statusCode: StatusCodes.Status409Conflict);

        var user = await context.Users
            .Include(c => c.Role)
            .FirstOrDefaultAsync(c => c.Phone == ticket.Login || c.Email == ticket.Login);
        if (user is null)
            return TypedResults.Text($"Пользователь с логином {ticket.Login} не найден.", statusCode: StatusCodes.Status404NotFound);

        user.RefreshToken = jwtHelper.CreateRefreshToken();
        user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenLifetime);
        context.Users.Update(user);
        ticket.IsUsed = true;
        context.AuthTickets.Update(ticket);
        await context.SaveChangesAsync();

        var authCompletedDto = new TokensDto
        {
            AccessToken = jwtHelper.CreateAccessToken(user, jwtOptions.AccessTokenLifetime, ticket.DeviceDescription),
            RefreshToken = user.RefreshToken,
        };
        
        return TypedResults.Ok(authCompletedDto);
    }
    
    private static async Task<IResult> RefreshTokens(DatabaseContext context,
        [FromBody] RefreshTokensDto refreshTokensDto,
        [FromServices] IJwtHelper jwtHelper,
        [FromServices] JwtOptions jwtOptions,
        HttpContext httpContext)
    {
        if (string.IsNullOrEmpty(refreshTokensDto.RefreshToken))
            return TypedResults.Text("Не указан токен обновления.", statusCode: StatusCodes.Status400BadRequest);

        var user = await context.Users
            .Include(c => c.Role)
            .FirstOrDefaultAsync(c => c.RefreshToken == refreshTokensDto.RefreshToken);
        if (user is null)
            return TypedResults.Text("Пользователь не найден.", statusCode: StatusCodes.Status404NotFound);

        if (user.RefreshTokenExpires < DateTime.UtcNow)
            return TypedResults.Text("Токен обновления просрочен.", statusCode: StatusCodes.Status410Gone);

        user.RefreshToken = jwtHelper.CreateRefreshToken();
        user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenLifetime);
        context.Users.Update(user);
        await context.SaveChangesAsync();

        var tokensDto = new TokensDto
        {
            AccessToken = jwtHelper.CreateAccessToken(user, jwtOptions.AccessTokenLifetime, refreshTokensDto.DeviceDescription),
            RefreshToken = user.RefreshToken,
        };
        
        return TypedResults.Ok(tokensDto);
    }
}
