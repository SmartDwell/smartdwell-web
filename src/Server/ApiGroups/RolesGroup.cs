using Contracts.Roles;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Seljmov.Blazor.Identity.Shared;
using Shared;

namespace Server.ApiGroups;

/// <summary>
/// Группа ролей.
/// </summary>
public static class RolesGroup
{
    /// <summary>
    /// Конфигурация группы.
    /// </summary>
    /// <param name="endpoints">Маршруты.</param>
    public static void MapRolesGroup(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup(RouteConstants.RolesData.Route);
            //.RequireAuthorization(AuthPolicies.AdminPolicy);
        group.MapGet(RouteConstants.RolesData.Roles, GetRoles)
            .Produces<RoleDto[]>()
            .WithName("GetRoles")
            .WithSummary("Получение списка ролей.")
            .WithOpenApi();
        group.MapPost(RouteConstants.RolesData.Add, AddRole)
            .Produces<IResult>()
            .WithName("AddRole")
            .WithSummary("Добавление роли.")
            .WithOpenApi();
    }
    
    private static async Task<Ok<List<RoleDto>>> GetRoles(DatabaseContext context)
    {
        var roles = await context.Roles
            .Include(role => role.Users)
            .Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Policies = role.Policies,
                UsersCount = role.Users.Count,
            })
            .ToListAsync();
        return TypedResults.Ok(roles);
    }
    
    private static async Task<IResult> AddRole(DatabaseContext context, [FromBody] RoleAddDto roleDto)
    {
        var existingRole = await context.Roles.FirstOrDefaultAsync(role => role.Name.Trim().ToLower() == roleDto.Name.Trim().ToLower());
        if (existingRole is not null)
            return TypedResults.Text("Роль с таким именем уже существует.", statusCode: StatusCodes.Status400BadRequest);
        
        var role = new Role
        {
            Name = roleDto.Name,
            Policies = roleDto.Policies,
        };
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
        return TypedResults.Ok(role);
    }
}
