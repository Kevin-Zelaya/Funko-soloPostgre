using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using StackExchange.Redis;

public class DependencyProvider
{
    public static void Configure(
        IServiceCollection _services
    )
    {
        Log.Information("[DEPENDENCY-PROVIDER] Configurando la inyección de dependencias.");
        _services.Scan(scan => scan
            .FromAssemblyOf<FunkoController>()
            .AddClasses(classes => classes.AssignableTo<IScoped>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingleton>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransient>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        /// <summary> Configuración de postgres </summary>
        Log.Information("[DEPENDENCY-PROVIDER] Aplicando proveedor segun perfil");
        _services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(AppConfig.ConnectionString));
        ;
              
        ////<summary> Configuración de redis </summary>
        var redis = ConnectionMultiplexer.Connect(
            AppConfig.RedisConnection
        );
        _services.AddSingleton<IConnectionMultiplexer>(redis);

    }
}