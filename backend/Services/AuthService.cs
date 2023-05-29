using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class AuthService : IAuthService
{
  private readonly IConfiguration _config;
  private readonly AuthContext _db;

  public AuthService(IConfiguration config, AuthContext db)
  {
    _config = config;
    _db = db;
  }

  public string GenerateUserToken(User user)
  {
    var issuer = _config["JwtSettings:Issuer"];
    var audience = _config["JwtSettings:Audience"];

    var claims = _db.UserGroups.Where(userGroup => userGroup.UserId == user.Id)
      .Select(userGroup => userGroup.Group)
      .Select(group => new Claim("groups", group!.Name))
      .ToList();

    var notBefore = DateTime.Now;
    var expires = DateTime.Now.AddHours(2);

    var signingKeyData = _config["JwtSettings:SigningKey"];
    var signingKeyBytes = Encoding.UTF8.GetBytes(signingKeyData!);
    var signingKey = new SymmetricSecurityKey(signingKeyBytes);

    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    var jwtToken = new JwtSecurityToken(
      issuer,
      audience,
      claims,
      notBefore,
      expires,
      signingCredentials
    );

    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);
    return encodedJwt;
  }
}