using Microsoft.EntityFrameworkCore;
using Models;
using Seljmov.Blazor.Identity.Shared;

namespace Server;

/// <summary>
/// Контекст базы данных.
/// </summary>
public sealed class DatabaseContext : DbContext
{
    #region Tables

    /// <summary>
    /// Коллекция тикетов авторизации.
    /// </summary>
    public DbSet<AuthTicket> AuthTickets { get; init; } = null!;
    
    /// <summary>
    /// Коллекция пользователей.
    /// </summary>
    public DbSet<User> Users { get; init; } = null!;

    /// <summary>
    /// Коллекция ролей.
    /// </summary>
    public DbSet<Role> Roles { get; init; } = null!;
    
    #endregion
    
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public DatabaseContext() { }
    
    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="options">Параметры.</param>
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
    {
        if (Database.GetPendingMigrations().Any())
            Database.Migrate();
    }

    /// <inheritdoc cref="DbContext.OnModelCreating(ModelBuilder)"/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.NewGuid();
        var residentRoleId = Guid.NewGuid();
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Policies).IsRequired();

            entity.HasMany(e => e.Users).WithOne(e => e.Role).HasForeignKey(e => e.RoleId);
            
            entity.HasData(new List<Role>
            {
                new()
                {
                    Id = adminRoleId,
                    Name = "Администратор",
                    Policies = AuthPolicies.AllPolicies.ToList(),
                },
                new()
                {
                    Id = residentRoleId,
                    Name = "Житель",
                    Policies =
                    [
                        AuthPolicies.AuthPolicy,
                        AuthPolicies.UsersPolicy,
                        AuthPolicies.RequestsPolicy,
                        AuthPolicies.NewsPolicy
                    ]
                }
            });
        });
        
        modelBuilder.Entity<AuthTicket>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.Login).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsUsed).IsRequired().HasDefaultValue(false);
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Surname).IsRequired();
            entity.Property(e => e.Patronymic);
            entity.Property(e => e.Phone).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.RoleId).IsRequired();
            entity.Property(e => e.Note);
            entity.Property(e => e.RefreshToken);
            entity.Property(e => e.RefreshTokenExpires);
            entity.Property(e => e.IsBlocked).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.BlockReason);
            entity.Property(e => e.Created).IsRequired().HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Role).WithMany(e => e.Users).HasForeignKey(e => e.RoleId);
            
            entity.HasData(new List<User>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    Note = "Создано автоматически",
                    Phone = "79887897788",
                    Email = "17moron@bk.ru",
                    Name = "Администратор",
                    Surname = "Тестовый",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    RoleId = residentRoleId,
                    Note = "Создано автоматически",
                    Phone = "79887893311",
                    Email = "guest@example.com",
                    Name = "Житель",
                    Surname = "Тестовый",
                }
            });
        });
    }
}