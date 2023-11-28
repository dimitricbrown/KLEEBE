using Microsoft.EntityFrameworkCore;
using KLEEBE.Models;

public class KLEEDbContext : DbContext
{
    public DbSet<Categories> Categories { get; set; }
    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<Restaurants> Restaurants { get; set; }
    public DbSet<Users> Users { get; set; }

    public KLEEDbContext(DbContextOptions<KLEEDbContext> context) : base(context)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
