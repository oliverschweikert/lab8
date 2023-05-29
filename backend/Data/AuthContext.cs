using Microsoft.EntityFrameworkCore;
using backend.Models;

public class AuthContext : DbContext
{
  public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

  public DbSet<User> Users { get; set; } = default!;
  public DbSet<Group> Groups { get; set; } = default!;
  public DbSet<UserGroup> UserGroups { get; set; } = default!;
  public DbSet<SuperSecureImage> SuperSecureImages { get; set; } = default!;

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.Entity<User>().HasData(new User
    {
      Id = 1,
      Email = "admin@admin.admin",
      Password = "admin"
    },
    new User
    {
      Id = 2,
      Email = "user@user.user",
      Password = "user"
    });

    builder.Entity<Group>().HasData(
        new Group
        {
          GroupId = 1,
          Name = "admin"
        },
        new Group
        {
          GroupId = 2,
          Name = "user"
        }
    );

    builder.Entity<UserGroup>().HasData(
        new UserGroup
        {
          Id = 1,
          UserId = 1,
          GroupId = 1
        },
        new UserGroup
        {
          Id = 2,
          UserId = 2,
          GroupId = 2
        }
    );
    builder.Entity<SuperSecureImage>().HasData(
        new SuperSecureImage
        {
          Id = 1,
          Name = "Image 1",
          Size = 100000,
          Url = "http://localhost:5000/thiswontwork"
        }
    );
  }
}
