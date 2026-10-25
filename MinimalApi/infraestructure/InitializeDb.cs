using MongoDB.Driver;
using Serilog;

public static class InitializationDb
{
    public static async Task InitializationDatabaseAsync(
        this WebApplication app,
        string contextType)
    {
        switch (contextType.ToLowerInvariant())
        {
            case "postgre":
            case "mongoef":
                {
                    using var scope = app.Services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    ///<summary> Eliminar base de datos <summary>
                    Log.Debug($"[{AppConfig.Environment.ToUpper()}] Eliminando y recreando la base de datos");
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                    Log.Information("[POSGRE] Eliminando y recreando la base de datos");
                    break;
                }     
            case "mongo":
                {
                    Log.Warning("[MONGODB] Falta por aqui");
                    break;
                }
            default:
                throw new InvalidOperationException(
                    $"Proveedor no soportado: {contextType}");
        }
    }
}