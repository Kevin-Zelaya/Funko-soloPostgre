using CSharpFunctionalExtensions;

public class CategoryService(
    ICategoryRepository _repository,
    ILogger<CategoryService> _logger,
    ICache _cache
) : ICategoryService, IScoped
{
    /// <inheritdoc cref="ICategoryService.GetAllCategoriesAsync(CancellationToken)"/>
    public async Task<Result<List<CategoryDto>, DomainError>> GetAllCategoriesAsync(
        CancellationToken ct
    )
    {
        _logger.LogDebug("[CATEGORY-GETALL] Intentando obtener la lista completa de categorias.");
        var response = await _repository.GetAllCategoriesAsync(ct);
        return Result.Success<List<CategoryDto>, DomainError>(
            response.Select(f => f.ToDto()).ToList()
        )
        .Tap(c => _logger.LogInformation("[CATEGORY-GETALL] Retornando lista completa de categorias."));
    }
    /// <inheritdoc cref="ICategoryService.GetPaginatedCategoriesAsync(int, int, CancellationToken)"/>
    public async Task<Result<PaginatedResouce<CategoryDto>, DomainError>> GetPaginatedCategoriesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        string key = $"SUPPLIER:ALL-PAGED:{pageNumber}"; 
        _logger.LogDebug("[CATEGORY-GETPAGINATED] Intentando obtener la lista páginada de categorias.");
        //1. Buscar en la caché
        if(pageNumber == 1)
        {
            var cached = await _cache.GetTAsync<PaginatedResouce<CategoryDto>>(key);
            if(cached is not null)
                return Result.Success<PaginatedResouce<CategoryDto>, DomainError>(
                    cached
                )
                .Tap(c => _logger.LogInformation("[CATEGORY-GETPAGINATED] Retornando lista páginada de categorias desde la caché."));
        }
        // 2. Buscar en la base de datos
        var response = await _repository.GetPaginatedCategoriesAsync(pageNumber, pageSize, ct);
        // 3 Transformar respuesta
        var paginated = new PaginatedResouce<CategoryDto>(
                        response.items.Select(c => c.ToDto()).ToList(),
                        response.count,
                        response.totalPages,
                        response.HasPreviousPage,
                        response.HasNextPage
            );
        // 4.Guardar en caché
        try{await _cache.SetAsync(key, paginated);}
        catch (System.Exception ex){_logger.LogWarning(ex, "[CATEGORY-GET-PAGED] Error al guardar lista páginada en la caché");}
        return Result.Success<PaginatedResouce<CategoryDto>, DomainError>(
            paginated
        )
        .Tap(c => _logger.LogInformation("[CATEGORY-GETPAGINATED] Retornando lista páginada de categorias."));
    }
    /// <inheritdoc cref="ICategoryService.GetCategoryByNameAsync(string, CancellationToken)"/>
    public async Task<Result<CategoryDto?, DomainError>> GetCategoryByNameAsync(
        string name,
        CancellationToken ct
    )
    {
        string key = $"CATEGORY:{name}";
        _logger.LogDebug("[CATEGORY-GETBYNAME] Intentando obtener la lista páginada de categorias.");
        // 1. Buscar en la caché.
        var cached = await _cache.GetTAsync<Category>(key);
        if(cached is not null) return Result.Success<CategoryDto?, DomainError>(
                    cached.ToDto())
                .Tap(c => _logger.LogInformation("[CATEGORY-GET] Retornando lista páginada de categorias."));
        // 2. Buscar en la base de datos.
        var response = await _repository.GetCategoryByName(name, ct);
        // 3. Guardar en la caché.
        if(response is not null) await _cache.SetAsync<Category>(key, response);
        return response is not null
            ? Result.Success<CategoryDto?, DomainError>(
                response.ToDto())
                .Tap(c => _logger.LogInformation("[CATEGORY-GETPAGINATED] Retornando lista páginada de categorias."))
            : Result.Failure<CategoryDto?, DomainError>(
                NotFoundError.FromResource("Category", ""))
                .Tap(c => _logger.LogError("[CATEGORY-GETBYNAME] No se ha encontrado la categoria con el nombre {name}", name));
    }
}