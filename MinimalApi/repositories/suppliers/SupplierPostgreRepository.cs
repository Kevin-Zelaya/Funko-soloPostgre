using Microsoft.EntityFrameworkCore;

public class SupplierPostgreRepository(
    AppDbContext _context
) : ISupplierRepository, IScoped
{
    
    /// <inheritdoc cref="ISupplierRepository.GetAllSuppliersAsync(CancellationToken)"/>
    public async Task<List<Supplier>> GetAllSuppliersAsync(
        CancellationToken ct = default
    )
    {
        return await _context.Suppliers
            .AsNoTracking()
            .ToListAsync(ct);
    }

    /// <inheritdoc cref="ISupplierRepository.GetPaginatedSuppliersAsync(int, int, CancellationToken)"/>
        public async Task<(
        List<Supplier> items,
        int count,
        int totalPages,
        bool HasPreviousPage,
        bool HasNextPage
    )> GetPaginatedSuppliersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = _context.Suppliers
            .AsNoTracking()
            .OrderBy(s => s.Id);

        // Total de proveedores disponibles
        int count = await query.CountAsync(ct);

        // Número total de páginas
        int totalPages = (int)Math.Ceiling((double)count / pageSize);

        // Proveedores de la página solicitada
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // Metadatos de paginación
        bool hasPreviousPage = pageNumber > 1;
        bool hasNextPage = pageNumber < totalPages;

        return (
            items,
            count,
            totalPages,
            hasPreviousPage,
            hasNextPage
        );
    }

    /// <inheritdoc cref="ISupplierRepository.GetSupplierByIdAsync(string, CancellationToken)"/>
    public async Task<Supplier?> GetSupplierByIdAsync(
        string id,
        CancellationToken ct = default
    )
    {
        return await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}