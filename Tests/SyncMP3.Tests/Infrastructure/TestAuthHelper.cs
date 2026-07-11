using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class TestAuthHelper
{
    public static string GenerateToken(Guid userId, UserType userType = UserType.Anonymous)
    {
        var claims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim("type", userType.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFactory.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: ApiFactory.JwtIssuer,
            audience: ApiFactory.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}