using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// ======= Logger de Serilog =============
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.AddSerilog(Log.Logger);

Log.Information("[PROGRAM] Aplicada la configuración de serilog mediante 'appsettings.json'.");

// ======= Registrando contoladores ========== 
builder.Services.AddControllers();

/// ==============
/// Proveedor de dependencias con scrutor y
/// configuración según perfíl
/// =============== 
DependencyProvider.Configure(
    builder.Services);

var app = builder.Build();
app.UseHttpsRedirection();

// ======= Configuración de open api ========== 
app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Mi API V1");
});

// ======= Registar controlador de excepciones ========== 
app.UseGlobalExceptionHandler();
// ======= Buscar todos los controladores ========== 
app.MapControllers();
// ======= Inicializar estructura y db en caso de postgre ========== 
if(AppConfig.shouldRecreateDb)
    await app.InitializationDatabaseAsync(AppConfig.Environment);

// Falta vaciar la bd en el caso de mongo
Console.WriteLine("======================================================");
Console.WriteLine("=================     FUNKO API     ==================");
Console.WriteLine("======================================================");
Console.WriteLine("");
Console.WriteLine("+===================+================================== ");
Console.WriteLine($"| api_environment   |      {AppConfig.Environment}    |");
Console.WriteLine($"|  swagger_route    |localhost:5205/swagger/index.html|");
Console.WriteLine($"|   Recreate_db     |                {(AppConfig.shouldRecreateDb ? "true " : "false")}            |");
Console.WriteLine($"+===================+=================================+");



app.Run();