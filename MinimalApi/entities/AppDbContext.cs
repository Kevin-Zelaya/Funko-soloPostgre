using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Funko> Funkos {get; set;}
    public DbSet<Supplier> Suppliers {get; set;}
    public DbSet<Category> Categories {get; set;}

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        PosgreContextConfiguration.Configure(modelBuilder);
    }
    
}