using Contracts.Roles;
using Contracts.Users;
using Mapster;
using Models;

namespace Server;

/// <summary>
/// Конфигурация Mapster.
/// </summary>
internal static class MapsterConfig
{
    /// <summary>
    /// Конфигурация.
    /// </summary>
    public static void Config()
    {
        TypeAdapterConfig<Role, RoleBaseDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
        
        TypeAdapterConfig<RoleBaseDto, Role>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
        
        TypeAdapterConfig<Role, RoleDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Policies, src => src.Policies)
            .Map(dest => dest.UsersCount, src => src.Users.Count);
        
        TypeAdapterConfig<RoleAddDto, Role>.NewConfig()
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Policies, src => src.Policies);
        
        TypeAdapterConfig<User, UserDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Surname, src => src.Surname)
            .Map(dest => dest.Patronymic, src => src.Patronymic)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.Role, src => src.Role.Adapt<RoleBaseDto>());
        
        TypeAdapterConfig<UserPutDto, User>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Surname, src => src.Surname)
            .Map(dest => dest.Patronymic, src => src.Patronymic)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.Role, src => src.Role.Adapt<Role>());
    }
}
