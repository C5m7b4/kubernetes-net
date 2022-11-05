using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
      Console.WriteLine("constructor");
    }

    public DbSet<Platform> Platforms { get; set; }
  }
}