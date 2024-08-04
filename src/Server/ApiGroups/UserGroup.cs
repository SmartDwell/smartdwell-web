using Contracts.Users;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Seljmov.Blazor.Identity.Shared;
using Shared;

namespace Server.ApiGroups;

/// <summary>
/// Группа методов для работы с пользователями.
/// </summary>
public static class UserGroup
{
    /// <summary>
    /// Определение маршрутов работы с пользователями.
    /// </summary>
    /// <param name="endpoints">Маршруты.</param>
    public static void MapUserGroup(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup(RouteConstants.UserData.Route)
            .RequireAuthorization(AuthPolicies.UsersPolicy);
        group.MapGet(RouteConstants.UserData.Users, GetUsers)
            .Produces<UserDto[]>()
            .WithName("GetUsers")
            .WithSummary("Получение списка пользователей.")
            .WithOpenApi();
        group.MapPut(RouteConstants.UserData.Put, PutUser)
            .Produces<IResult>()
            .WithName("PutUser")
            .WithSummary("Добавление/редактирование пользователя.")
            .WithOpenApi();
        group.MapDelete(RouteConstants.UserData.Delete, DeleteUser)
            .Produces<IResult>()
            .WithName("DeleteUser")
            .WithSummary("Удаление пользователя.")
            .WithOpenApi();
        group.MapGet(RouteConstants.UserData.UserById, GetUserById)
            .Produces<UserDto>()
            .WithName("GetUsersById")
            .WithSummary("Получение пользователя по идентификатору.")
            .WithOpenApi();
    }

    private static Ok<UserDto[]> GetUsers(DatabaseContext context)
    {
        var users = context.Users
            .Include(user => user.Role)
            .Adapt<UserDto[]>();
        return TypedResults.Ok(users);
    }
    
    private static async Task<IResult> PutUser(DatabaseContext context, [FromBody] UserPutDto userDto)
    {
        if (!context.Roles.Any(role => role.Id == userDto.Role.Id))
            return TypedResults.Text("Роль не найдена.", statusCode: StatusCodes.Status404NotFound);
        
        return userDto.Id.HasValue ? await EditUser(userDto) : await AddUser(userDto);

        async Task<IResult> AddUser(UserPutDto userPutDto)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(user => user.Email == userPutDto.Email || user.Phone == userPutDto.Phone);
            if (existingUser is not null)
                return TypedResults.Text("Пользователь с таким номером телефона или электронной почтой уже существует.", statusCode: StatusCodes.Status400BadRequest);
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = userPutDto.Name,
                Surname = userPutDto.Surname,
                Patronymic = userPutDto.Patronymic,
                Email = userPutDto.Email,
                Phone = userPutDto.Phone,
                RoleId = userPutDto.Role.Id
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
         
            return await GetUserById(context, user.Id);
        }

        async Task<IResult> EditUser(UserPutDto userPutDto)
        {
            var user = await context.Users
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.Id == userPutDto.Id);
        
            if (user is null)
                return TypedResults.Text("Пользователь не найден.", statusCode: StatusCodes.Status404NotFound);
        
            user.Name = userPutDto.Name;
            user.Surname = userPutDto.Surname;
            user.Patronymic = userPutDto.Patronymic;
            user.Email = userPutDto.Email;
            user.Phone = userPutDto.Phone;
            user.Role.Id = userPutDto.Role.Id;
        
            context.Users.Update(user);
            await context.SaveChangesAsync();
            
            return await GetUserById(context, user.Id);
        }
    }

    private static async Task<IResult> DeleteUser(DatabaseContext context, [FromRoute] Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == id);
        if (user is null)
            return TypedResults.Text("Пользователь не найден.", statusCode: StatusCodes.Status404NotFound);
        
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }
    
    private static async Task<IResult> GetUserById(DatabaseContext context, [FromRoute] Guid id)
    {
        var user = await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id == id);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user.Adapt<UserDto>());
    }
}
