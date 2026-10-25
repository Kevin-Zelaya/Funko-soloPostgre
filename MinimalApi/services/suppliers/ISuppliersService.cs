using CSharpFunctionalExtensions;

public interface ISupplierService
{
    /// <summary>
    /// Obtiene todos los proveedores.
    /// </summary>
    public Task<Result<List<SupplierDto>, DomainError>> GetAllSuppliersAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Obtiene los proveedores de una página.
    /// </summary>
    public Task<Result<PaginatedResouce<SupplierDto>, DomainError>> GetPaginatedSuppliersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    /// <summary>
    /// Obtiene un proveedor por su identificador.
    /// </summary>
    Task<Result<SupplierDto, DomainError>> GetSupplierByIdAsync(
        string id,
        CancellationToken ct = default);
}