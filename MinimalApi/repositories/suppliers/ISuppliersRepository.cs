public interface ISupplierRepository
{
    /// <summary>
    /// Obtiene todos los proveedores.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista de proveedores.</returns>
    Task<List<Supplier>> GetAllSuppliersAsync(
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtiene una lista paginada de proveedores.
    /// </summary>
    /// <param name="pageNumber">Número de página.</param>
    /// <param name="pageSize">Cantidad de proveedores por página.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista paginada de proveedores.</returns>
    Task<(
        List<Supplier> items,
        int count,
        int totalPages,
        bool HasPreviousPage,
        bool HasNextPage
    )> GetPaginatedSuppliersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtiene un proveedor por su ID.
    /// </summary>
    /// <param name="id">Identificador del proveedor.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>El proveedor encontrado o null si no existe.</returns>
    Task<Supplier?> GetSupplierByIdAsync(
        string id,
        CancellationToken ct = default
    );
}