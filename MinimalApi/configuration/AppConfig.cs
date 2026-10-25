public static class AppConfig
{
    static AppConfig()
    {
        Environment = 
            System.Environment
                .GetEnvironmentVariable("DOTNET_ENVIRONMENT")?
                .ToLowerInvariant() 
            ?? "postgre";
            
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true
            )
            .AddJsonFile(
                 $"appsettings.{(Environment is "mongoef" or "mongo" ? "mongo" : Environment)}.json",
                optional: false,
                reloadOnChange: true
            )
            .Build();
        ConnectionString = Configuration["ConnectionStrings:DefaultConnection"]
            ?? throw new ArgumentException("Error al obtener la propiedad ConnectionStrings:DefaultConnection");
        shouldRecreateDb = Configuration.GetValue<bool?>("shouldRecreateDb") 
            ?? throw new ArgumentException("Error al obtener la propiedad shouldRecreateDb");
        RedisConnection = Configuration["Redis:ConnectionString"]
            ?? throw new ArgumentException("Error al obtener la propiedad shouldRecreateDb");
    }

    public static IConfiguration Configuration {get; private set;}
    public static string ConnectionString {get; private set;}

    public static string Environment {get; private set;}
    public static bool shouldRecreateDb {get; private set;}
    public static string RedisConnection {get; private set;}
} 