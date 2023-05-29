namespace backend.Models;

public class SuperSecureImage
{
  public int Id { get; set; }
  public string Url { get; set; } = default!;
  public string Name { get; set; } = default!;
  public int Size { get; set; }
}