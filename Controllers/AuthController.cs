using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.Auth.Models;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration config, ILogger<AuthController> logger)
    {
        _logger = logger;
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        if (login.Username == "admin" && login.Password == "password") // TODO replace with real validation
        {
            var token = GenerateJwtToken(login.Username);
            _logger.LogInformation("Login was successful");
            return Ok(new { token });
        }

        return Unauthorized();
    }

    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "key"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new[]
        {
            new Claim("sub", username),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        var handler = new JsonWebTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = "TaskManager",
            Audience = "TaskManagerUsers",
            Claims = claims.ToDictionary(claim => (string)claim.Type, claim => (object)claim.Value),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials
        };

        return handler.CreateToken(tokenDescriptor);
    }
}

