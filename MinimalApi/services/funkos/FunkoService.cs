
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class FunkoService(
    IFunkoRepository _repository,
    ILogger<FunkoService> _logger,
    ICache _cache
) : IFunkoService, IScoped
{
    // =========================================================
    // CREATE
    // =========================================================

    /// <inheritdoc cref="IFunkoService.CreateFunkoAsync(FunkoCreateRequestDto, CancellationToken)"/>
    public async Task<Result<FunkoDto, DomainError>> CreateFunkoAsync(
        FunkoCreateRequestDto funko,
        CancellationToken ct = default)
    {
        _logger.LogDebug(
            "[SERVICE-CREATE] Intentando crear un Funko.");
        // Comprobar si existe el proveedor
        if(!await _repository.ExistSupplierByIdAsync(funko.SupplierId, ct))
            return Result.Failure<FunkoDto, DomainError>(
                NotFoundError.FromResource("Supplier", funko.SupplierId));
        // Crear funko
        var response = await _repository.CreateFunkoAsync(
            funko.ToModel(), ct);
        // Retornar como dto
        return Result.Success<FunkoDto, DomainError>(response.ToDto())
            .Tap(f => _logger.LogInformation(
                "[SERVICE-CREATE] Funko creado correctamente con ID {Id}.",
                f.Id));
    }

    // =========================================================
    // READ
    // =========================================================

    /// <inheritdoc cref="IFunkoService.GetFunkoByIdAsync(string, CancellationToken)"/>
    public async Task<Result<FunkoDto, DomainError>> GetFunkoByIdAsync( 
        string id,
        CancellationToken ct)
    {
        // Llave para buscar en caché
        string key = $"FUNKO:{id}";
        _logger.LogDebug(
            "[SERVICE-GET] Buscando Funko con ID {Id}.", id);

        // 1. Buscar en caché
        var cached = await _cache.GetTAsync<Funko>(key);
        if(cached is not null) 
            Result.Success<FunkoDto, DomainError>(cached.ToDto())
                .Tap(f => _logger.LogInformation(
                    "[SERVICE-GET] Funko con ID {Id} recuperado correctamente desde la cache.", id));
        // buscar en la bd
        var response = await _repository.GetFunkoByIdAsync(id, ct);
        if(response is null)
            return Result.Failure<FunkoDto, DomainError>(
                    FunkoError.NotFound(id))
                    .Tap(_ => _logger.LogWarning(
                        "[SERVICE-GET] No se encontró el Funko con ID {Id}.",
                        id)); 
        // Agregar a la caché
        try{await _cache.SetAsync(key, response);
        await _cache.RemoveAsync("FUNKO:ALL-PAGINATED:1"); }// Eliminar páginación cacheada
        catch(Exception ex){_logger.LogWarning(ex, "No se pudo guardar el funko {id} en la cache", id);}
        // Retornar si todo sale bien
        return Result.Success<FunkoDto, DomainError>(response.ToDto())
                .Tap(f => _logger.LogInformation(
                    "[SERVICE-GET] Funko con ID {Id} recuperado correctamente.", id));
    }

    /// <inheritdoc cref="IFunkoService.GetAllFunkos(CancellationToken)"/>
    public async Task<Result<List<FunkoDto>, DomainError>> GetAllFunkos(
        CancellationToken ct = default)
    {
        _logger.LogDebug(
            "[SERVICE-GETALL] Recuperando todos los Funkos.");

        var response = await _repository.GetAllFunkosAsync(ct);

        return Result.Success<List<FunkoDto>, DomainError>(
            response.Select(f => f.ToDto()).ToList())
            .Tap(_ => _logger.LogInformation(
                "[SERVICE-GETALL] Se recuperaron {Count} Funkos.",
                response.Count));
    }

    /// <inheritdoc cref="IFunkoService.GetPaginatedFunkos(string, string, CancellationToken)"/>
    public async Task<Result<PaginatedResouce<FunkoDto>, DomainError>> GetPaginatedFunkos(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        // Llave para paginación
        string key = $"FUNKO:ALL-PAGINATED:{pageNumber}";
        _logger.LogDebug(
            "[SERVICE-GETPAGINATED] Recuperando página {PageNumber} con tamaño {PageSize}.",
            pageNumber, pageSize);
        if(pageNumber == 1) // Si es la página 1, busca en caché
        {
            var cached = await _cache.GetTAsync<PaginatedResouce<FunkoDto>>(key);
            if(cached is not null) // si lo encuentra retorna directamente
                return Result.Success<PaginatedResouce<FunkoDto>, DomainError>(
                    cached)
                .Tap(_ => _logger.LogInformation(
                    "[SERVICE-GETPAGINATED] Se recuperaron {Count} Funkos de la página {PageNumber} desde el caché.",
                        cached.Count, pageNumber));
        }
        // Buscar en la base de datos
        var response = await _repository.GetPaginatedFunkosAsync(
            pageNumber, pageSize, ct);
        // Transpofmar la respuesta en un dto que representa mejor la paginación
        var paginatedResponse = new PaginatedResouce<FunkoDto>(
                response.items.Select(f => f.ToDto()).ToList(),
                response.count,
                response.totalPages,
                response.HasPreviousPage,
                response.HasNextPage
            );
        // Guardar en caché solo si es la página 1 y contiene elementos
        if(response.count > 0 && pageNumber == 1)
            await _cache.SetAsync(key, paginatedResponse);
        return Result.Success<PaginatedResouce<FunkoDto>, DomainError>(
                paginatedResponse)
            .Tap(_ => _logger.LogInformation(
                "[SERVICE-GETPAGINATED] Se recuperaron {Count} Funkos de la página {PageNumber}.",
                response.items.Count, pageNumber));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    /// <inheritdoc cref="IFunkoService.UpdateFunkoAsync(string, FunkoUpdateRequestDto, CancellationToken)"/>
    public async Task<Result<FunkoDto, DomainError>> UpdateFunkoAsync(
        string id,
        FunkoUpdateRequestDto request,
        CancellationToken ct = default)
    {
        string key = $"FUNKO:{id}";
        _logger.LogDebug(
            "[SERVICE-UPDATE] Intentando actualizar el Funko con ID {Id}.",
            id);
        // Actualizar en db
        var response = await _repository.UpdateFunkoAsync(id, request, ct);
        // Invalidar en caché
        await _cache.RemoveAsync(key);
        await _cache.RemoveAsync("FUNKO:ALL-PAGINATED:1"); // Eliminar páginación cacheada
        // retornar según si es nulo o no
        return response is not null
            ? Result.Success<FunkoDto, DomainError>(response.ToDto())
                .Tap(f => _logger.LogInformation(
                    "[SERVICE-UPDATE] Funko con ID {Id} actualizado correctamente.",
                    f.Id))
            : Result.Failure<FunkoDto, DomainError>(
                FunkoError.NotFound(id))
                .Tap(_ => _logger.LogWarning(
                    "[SERVICE-UPDATE] No se encontró el Funko con ID {Id}.",
                    id));
    }
    /// <inheritdoc cref="IFunkoService.UpdateCategoryFromAsync(string, CategoryUpdateRequestDto, CancellationToken)"/>
    public async Task<Result<FunkoDto, DomainError>> UpdateCategoryFromAsync(
        string id,
        CategoryUpdateRequestDto request,
        CancellationToken ct = default)
    {
        string key = $"FUNKO:{id}";
        _logger.LogDebug(
            "[SERVICE-UPDATECATEGORY] Intentando actualizar la categoría del Funko con el ID {Id}.",
            id);
        // Actualizar la categoría en la db
        var response = await _repository.UpdateCategoryFromFunkoAsync(id, request, ct);
        if(response is not null) // solo si lo encontró
        {
            // Invalidar en caché
            await _cache.RemoveAsync(key);
            await _cache.RemoveAsync("FUNKO:ALL-PAGINATED:1"); // Eliminar páginación cacheada
        }
        return response is not null
            ? Result.Success<FunkoDto, DomainError>(response.ToDto())
                .Tap(f => _logger.LogInformation(
                    "[SERVICE-UPDATECATEGORY] Funko con ID {Id} actualizado correctamente.",
                    f.Id))
            : Result.Failure<FunkoDto, DomainError>(
                FunkoError.NotFound(id))
                .Tap(_ => _logger.LogWarning(
                    "[SERVICE-UPDATECATEGORY] No se encontró el Funko con ID {Id}.",
                    id));
    }
    /// <inheritdoc cref="IFunkoService.UpdateSupplierFromFunko(string, string, CancellationToken)"/>
    public async Task<Result<FunkoDto, DomainError>> UpdateSupplierFromFunko(
        string id,
        string supplierId,
        CancellationToken ct = default)
    {
        string key = $"FUNKO:{id}";
        _logger.LogDebug(
            "[SERVICE-UPDATECATEGORY] Intentando actualizar la categoría del Funko con el ID {Id}.",
            id);
        // Verificar si existe el proveedor
        if(!await _repository.ExistSupplierByIdAsync(supplierId, ct))
            return Result.Failure<FunkoDto, DomainError>(
                NotFoundError.FromResource("Supplier", supplierId));
        // Actualizar en la base de datos
        var response = await _repository.UpdateSuplierFromFunkoAsync(id, supplierId, ct);
        if(response is not null) // solo si lo encontró
        {// Invalidar en caché
            await _cache.RemoveAsync(key);
            await _cache.RemoveAsync("FUNKO:ALL-PAGINATED:1"); // Eliminar páginación cacheada
        }
        return response is not null
            ? Result.Success<FunkoDto, DomainError>(response.ToDto())
                .Tap(f => _logger.LogInformation(
                    "[SERVICE-UPDATESUPPLIER] Funko con ID {Id} actualizado correctamente.",
                    f.Id))
            : Result.Failure<FunkoDto, DomainError>(
                FunkoError.NotFound(id))
                .Tap(_ => _logger.LogWarning(
                    "[SERVICE-UPDATESUPPLIER] No se encontró el Funko con ID {Id}.",
                    id));
    }



    // =========================================================
    // DELETE / RESTORE
    // =========================================================

    /// <inheritdoc cref="IFunkoService.DeleteFunkoAsync(string, CancellationToken)"/>
    public async Task<Result<bool, DomainError>> DeleteFunkoAsync(
        string id,
        CancellationToken ct = default)
    {
        string key = $"FUNKO:{id}";
        _logger.LogDebug(
            "[SERVICE-DELETE] Intentando eliminar el Funko con ID {Id}.",
            id);
        // Eliminar en la base de datos
        var response = await _repository.DeleteFunkoAsync(id, ct);
        // Eliminar en la caché
        if(response) {
            await _cache.RemoveAsync(key);
            await _cache.RemoveAsync("FUNKO:ALL-PAGINATED:1"); // Eliminar páginación cacheada
        }
        // Retornar según si es verdadero o false la eliminación
        return response
            ? Result.Success<bool, DomainError>(true)
                .Tap(_ => _logger.LogInformation(
                    "[SERVICE-DELETE] Funko con ID {Id} eliminado correctamente.",
                    id))
            : Result.Failure<bool, DomainError>(
                FunkoError.NotFound(id))
                .Tap(_ => _logger.LogWarning(
                    "[SERVICE-DELETE] No se encontró el Funko con ID {Id}.",
                    id));
    }

    /// <inheritdoc cref="IFunkoService.RestoreDeletedFunkoAsync(string, CancellationToken)"/>
    public async Task<Result<FunkoDto, DomainError>> RestoreDeletedFunkoAsync(
        string id,
        CancellationToken ct = default)
    {
        _logger.LogDebug(
            "[SERVICE-RESTORE] Intentando restaurar el Funko con ID {Id}.",
            id);

        var response = await _repository.RestoreFunkoAsync(id, ct);

        return response is not null
            ? Result.Success<FunkoDto, DomainError>(response.ToDto())
                .Tap(f => _logger.LogInformation(
                    "[SERVICE-RESTORE] Funko con ID {Id} restaurado correctamente.",
                    f.Id))
            : Result.Failure<FunkoDto, DomainError>(
                FunkoError.NotFound(id))
                .Tap(_ => _logger.LogWarning(
                    "[SERVICE-RESTORE] No se encontró un Funko eliminado con ID {Id}.",
                    id));
    }

    // =========================================================
    // CATEGORIES
    // =========================================================

    /// <inheritdoc cref="IFunkoService.GetAllCategories(CancellationToken)"/>
    public async Task<Result<List<Category>, DomainError>> GetAllCategories(
        CancellationToken ct = default)
    {
        _logger.LogDebug(
            "[SERVICE-GETCATEGORIES] Recuperando todas las categorías.");

        var response = await _repository.GetAllCategoriesAsync(ct);

        return Result.Success<List<Category>, DomainError>(response)
            .Tap(_ => _logger.LogInformation(
                "[SERVICE-GETCATEGORIES] Se recuperaron {Count} categorías.",
                response.Count));
    }
}
