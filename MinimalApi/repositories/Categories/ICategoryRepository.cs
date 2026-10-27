
/// <summary>
/// Define las operaciones de acceso a datos para la gestión de categorías.
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
    /// Obtiene una lista paginada de categorías junto con sus metadatos de paginación.
    /// </summary>
    /// <param name="pageNumber">Número de la página que se desea obtener.</param>
    /// <param name="pageSize">Cantidad máxima de categorías por página.</param>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>
    /// Una tupla con las categorías de la página solicitada, el número de elementos
    /// devueltos, el número total de páginas y los indicadores de existencia de
    /// página anterior y siguiente.
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
    /// Crea una nueva categoría.
    /// </summary>
    /// <param name="request">Datos necesarios para crear la categoría.</param>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>La categoría creada.</returns>
    Task<Category> CreateCategoryAsync(
        CategoryCreateRequestDto request,
        CancellationToken ct
    );

    /// <summary>
    /// Marca una categoría como eliminada.
    /// </summary>
    /// <param name="id">Identificador de la categoría que se desea eliminar.</param>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>
    /// true si la operación se completa correctamente; en caso contrario, false.
    /// </returns>
    Task<bool> DeleteCategoryAsync(
        Guid id,
        CancellationToken ct
    );

    /// <summary>
    /// Restaura una categoría que ha sido eliminada lógicamente.
    /// </summary>
    /// <param name="id">Identificador de la categoría que se desea restaurar.</param>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>
    /// true si la operación se completa correctamente; en caso contrario, false.
    /// </returns>
    Task<bool> RestoreCategoryAsync(
        Guid id,
        CancellationToken ct
    );

    /// <summary>
    /// Actualiza los datos de una categoría existente.
    /// </summary>
    /// <param name="id">Identificador de la categoría que se desea actualizar.</param>
    /// <param name="request">Nuevos datos de la categoría, incluida la versión de concurrencia.</param>
    /// <param name="ct">Token de cancelación de la operación.</param>
    /// <returns>La categoría actualizada.</returns>
    Task<Category> UpdateCategoryAsync(
        Guid id,
        CategoryUpdateRequestDto request,
        CancellationToken ct
    );
}
