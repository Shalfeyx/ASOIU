using Microsoft.EntityFrameworkCore;
using RestaurantMenuApp.Models;

namespace RestaurantMenuApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=restaurant.db");
    }
}
