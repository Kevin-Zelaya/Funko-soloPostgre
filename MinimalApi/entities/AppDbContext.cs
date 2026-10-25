using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Funko> Funkos {get; set;}
    public DbSet<Supplier> Suppliers {get; set;}
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        // Para solucionar problemas con el ensurecreate
        // ya que da error por las transacciones
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

         switch (AppConfig.Environment.ToLowerInvariant())
        {
            case "postgre":
                PosgreContextConfiguration.Configure(modelBuilder);
                break;

            case "mongoef":
                MongoContextConfiguration.Configure(modelBuilder);
                break;

            default:
                throw new InvalidOperationException(
                    $"Entorno no soportado: {AppConfig.Environment}");
        }
    }
    
}