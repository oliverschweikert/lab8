using backend.Models;

namespace backend.Dtos;

public class SuperSecureImageResponse
{
  public SuperSecureImageResponse()
  {
  }

  public SuperSecureImageResponse(SuperSecureImage image)
  {
    Id = image.Id;
    Url = image.Url;
    Name = image.Name;
  }

  public int Id { get; set; }
  public string Url { get; set; } = default!;
  public string Name { get; set; } = default!;

  
}

