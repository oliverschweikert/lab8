using backend.Models;

namespace backend.Services;

public interface IAuthService
{
  public string GenerateUserToken(User user);
}