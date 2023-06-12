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
  
   public LoginResponse Login(LoginRequest request)
  {
    var response = new LoginResponse();
    var user = db.Users.Where(u => u.Email == request.Email).FirstOrDefault();
    if (user == null)
      return response.SetError("Email or password was incorrect");

    if (user.Password != request.Password)
      return response.SetError("Email or password was incorrect");

    var token = GenerateUserToken(user);

    return response.SetUserId(user.Id).SetToken(token);
  }

  public IActionResult Register(RegisterRequest request)
  {
    throw new NotImplementedException();
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
