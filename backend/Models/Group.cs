namespace backend.Models;

public class Group{
  public int GroupId { get; set; }
  public string Name { get; set; } = default!;
  public List<UserGroup> Users { get; set; } = new();
}