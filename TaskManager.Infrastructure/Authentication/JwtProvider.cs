using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace TaskManager.Infrastructure.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly string _secretKey;

    public JwtProvider(IConfiguration configuration)
    {
        // El Arquitecto lee de appsettings.json, pero dejamos un fallback para tu demo
        _secretKey = configuration["Jwt:SecretKey"] ?? "EstaEsUnaClaveSuperSecretaParaElTestTecnico123!";
    }

    public string Generate(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}