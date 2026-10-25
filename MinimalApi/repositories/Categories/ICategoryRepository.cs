/// <summary>
/// Define las operaciones de acceso a datos para las categorías.
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Obtiene todas las categorías disponibles.
    /// </summary>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>Lista con todas las categorías.</returns>
    Task<List<Category>> GetAllCategoriesAsync(
        CancellationToken ct
    );

    /// <summary>
    /// Busca una categoría por su nombre.
    /// </summary>
    /// <param name="name">Nombre de la categoría que se desea buscar.</param>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>La categoría encontrada o null si no existe.</returns>
    Task<Category?> GetCategoryByName(
        string name,
        CancellationToken ct
    );

    /// <summary>
    /// Obtiene una lista paginada de categorías junto con sus metadatos de paginación.
    /// </summary>
    /// <param name="pageNumber">Número de la página que se desea obtener.</param>
    /// <param name="pageSize">Cantidad máxima de categorías por página.</param>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>
    /// Una tupla con las categorías de la página solicitada, el número total de categorías,
    /// el número total de páginas y los indicadores de existencia de página anterior y siguiente.
    /// </returns>
    Task<(
        List<Category> items,
        int count,
        int totalPages,
        bool HasPreviousPage,
        bool HasNextPage
    )> GetPaginatedCategoriesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    );
}