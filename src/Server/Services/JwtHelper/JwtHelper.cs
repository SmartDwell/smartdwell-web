using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Seljmov.AspNet.Commons.Options;
using Models;

namespace Server.Services.JwtHelper;

/// <summary>
/// Интерфейс для генерации Jwt-токена
/// </summary>
public interface IJwtHelper
{
    /// <summary>
    /// Создать токен доступа.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <param name="minutesValid">Время действия токена.</param>
    /// <param name="deviceDescription">Описание устройства.</param>
    /// <returns>Токен доступа.</returns>
    string CreateAccessToken(User user, int minutesValid, string? deviceDescription = "");

    /// <summary>
    /// Создать токен обновления.
    /// </summary>
    /// <returns>Токен обновления.</returns>
    string CreateRefreshToken();
}

/// <summary>
/// Класс для генерации Jwt-токена
/// </summary>
public class JwtHelper : IJwtHelper
{
    private const int RefreshTokenLength = 64;
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// Конструктор класса <see cref="JwtHelper"/>
    /// </summary>
    /// <param name="jwtOptions">Настройки jwt</param>
    public JwtHelper(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }
    
    /// <inheritdoc cref="IJwtHelper.CreateAccessToken(User, int, string?)" />
    public string CreateAccessToken(User user, int minutesValid, string? deviceDescription = "")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Surname, user.Surname),
            new Claim(ClaimTypes.Role, string.Join(",", user.Role.Policies)),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.Phone),
            new Claim(ClaimTypes.System, deviceDescription ?? string.Empty),
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(minutesValid),
            signingCredentials: new SigningCredentials(_jwtOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    /// <inheritdoc cref="IJwtHelper.CreateRefreshToken" />
    public string CreateRefreshToken()
    {
        var token = RandomNumberGenerator.GetBytes(RefreshTokenLength);
        var bytes = Encoding.UTF8.GetBytes(Convert.ToBase64String(token));
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
