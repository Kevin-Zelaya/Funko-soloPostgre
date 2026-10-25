using CSharpFunctionalExtensions;

public interface ICategoryService
{
    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista de categorías o un error de dominio.</returns>
    Task<Result<List<CategoryDto>, DomainError>> GetAllCategoriesAsync(
        CancellationToken ct);

    /// <summary>
    /// Obtiene una categoría por su nombre.
    /// </summary>
    /// <param name="name">Nombre de la categoría.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Categoría encontrada o error de dominio.</returns>
    Task<Result<CategoryDto?, DomainError>> GetCategoryByNameAsync(
        string name,
        CancellationToken ct);

    /// <summary>
    /// Obtiene una lista paginada de categorías.
    /// </summary>
    /// <param name="pageNumber">Número de página solicitada.</param>
    /// <param name="pageSize">Cantidad de categorías por página.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Categorías paginadas con sus metadatos o un error de dominio.</returns>
    Task<Result<PaginatedResouce<CategoryDto>, DomainError>> GetPaginatedCategoriesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct);
}