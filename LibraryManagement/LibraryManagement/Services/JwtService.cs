
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Services;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;

    }

    public string GenerateAccessToken(string username, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public DateTime GenerateRefreshTokenExpiryTime()
    {
        return DateTime.UtcNow.AddDays(7);
    }

    // public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    // {
    //     var tokenValidationParameters = new TokenValidationParameters
    //     {
    //         ValidateAudience = true,
    //         ValidateIssuer = true,
    //         ValidateIssuerSigningKey = true,
    //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
    //         ValidIssuer = _config["Jwt:Issuer"],
    //         ValidAudience = _config["Jwt:Audience"],
    //         ValidateLifetime = false // Quan trọng: cho phép token hết hạn vẫn được verify
    //     };

    //     var tokenHandler = new JwtSecurityTokenHandler();

    //     try
    //     {
    //         var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

    //         if (securityToken is not JwtSecurityToken jwtSecurityToken ||
    //             !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
    //             StringComparison.InvariantCultureIgnoreCase))
    //         {
    //             return null;
    //         }

    //         return principal;
    //     }
    //     catch
    //     {
    //         return null;
    //     }
    // }
}
