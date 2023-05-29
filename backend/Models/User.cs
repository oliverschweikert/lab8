using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Index(nameof(Email), IsUnique = true)]
public class User{
 public int Id { get; set; }
 public string Email { get; set; } = default!;
 public string Password { get; set; } = default!;
 public List<UserGroup> Groups { get; set; } = new();
}