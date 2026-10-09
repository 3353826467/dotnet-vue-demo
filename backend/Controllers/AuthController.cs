using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Controllers;

public record RegisterDto(string Username, string Password);
public record LoginDto(string Username, string Password);

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Password.Length < 6)
            return BadRequest(new { message = "用户名不能为空，密码至少 6 位" });

        var exists = await db.Users.AnyAsync(u => u.Username == dto.Username);
        if (exists)
            return BadRequest(new { message = "用户名已被注册" });

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new { token = GenerateToken(user), username = user.Username });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "用户名或密码错误" });

        return Ok(new { token = GenerateToken(user), username = user.Username });
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var minutes = int.TryParse(config["Jwt:ExpireMinutes"], out var m) ? m : 720;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
