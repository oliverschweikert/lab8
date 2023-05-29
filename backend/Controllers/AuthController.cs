using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;


public class AuthController : BaseController
{
  private readonly AuthContext _db;
  private readonly IAuthService _auth;

  public AuthController(AuthContext db, IAuthService auth)
  {
    _db = db;
    _auth = auth;
  }

  [HttpPost]
  public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
  {
    var response = new LoginResponse();

    var user = _db.Users.Where(user => user.Email == request.Email).SingleOrDefault();
    if (user == null)
    {
      response.ErrorMessage = "Username or password was invalid";
      return new JsonResult(response);
    }
    var passwordIsCorrect = user.Password == request.Password;

    if (!passwordIsCorrect)
    {
      response.ErrorMessage = "Username or password was invalid";
      return new JsonResult(response);
    }

    var userToken = _auth.GenerateUserToken(user);

    if (userToken == null)
    {
      response.ErrorMessage = "Something went terribly wrong!";
      return new JsonResult(response);
    }

    response.Token = userToken;
    return new JsonResult(response);
  }
}