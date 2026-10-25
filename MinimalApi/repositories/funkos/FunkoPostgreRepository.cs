
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Serilog;

/// <summary>
/// Repositorio de acceso a datos de Funkos mediante PostgreSQL.
/// </summary>
/// 
/// TODO: Hacer control de concurrencia en los patch y updates
public class PosgreRepository(
    AppDbContext _context
) : IFunkoRepository, IScoped
{
    // =========================================================
    // CREATE
    // =========================================================

    /// <inheritdoc cref="IFunkoRepository.CreateFunkoAsync(Funko, CancellationToken)"/>
    public async Task<Funko> CreateFunkoAsync(
        Funko funko,
        CancellationToken ct = default)
    {
        /// Creamos id de la entidad para mantener
        /// conpatibilidad con mongo
        funko.Id = ObjectId.GenerateNewId().ToString();

        _context.Funkos.Add(funko);

        await _context.SaveChangesAsync(ct);

        await _context.Entry(funko) // Obtener el proveedor al que hace referencia
            .Reference(f => f.Supplier)
            .LoadAsync(ct);

        return funko;
    }

    // =========================================================
    // READ
    // =========================================================

    /// <inheritdoc cref="IFunkoRepository.GetFunkoByIdAsync(string, CancellationToken)"/>
    public async Task<Funko?> GetFunkoByIdAsync(
        string id,
        CancellationToken ct = default)
    {
        Funko? funko = await _context.Funkos
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id, ct);
        if(funko is null) return null;
        await _context.Entry(funko) // Obtener el proveedor al que hace referencia
            .Reference(f => f.Supplier)
            .LoadAsync(ct);
        return funko;
    } // 

    /// <inheritdoc cref="IFunkoRepository.GetAllFunkosAsync(CancellationToken)"/>
    public async Task<List<Funko>> GetAllFunkosAsync(CancellationToken ct = default)
    {
        // 1. Cargar colecciones founkos/Proveedores
        List<Funko> funkos = await _context.Funkos
            .AsNoTracking()
            .ToListAsync(ct);
        Dictionary<string, Supplier> suppliersDict = await _context.Suppliers
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Id, ct); // a diccionario para mejores busquedas.
        // 2. Asignar el proveedor a cada funko de forma directa
        foreach (var funko in funkos)
        {
            if (suppliersDict.TryGetValue(funko.SupplierId, out var supplier))
            {
                funko.Supplier = supplier;
            }
        }

        return funkos;
    }

    /// <inheritdoc cref="IFunkoRepository.GetPaginatedFunkosAsync(int, int, CancellationToken)"/>
    public async Task<(
        List<Funko> items,
        int count,
        int totalPages,
        bool HasPreviousPage,
        bool HasNextPage
    )> GetPaginatedFunkosAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        // 1. Cargar colecciones founkos/Proveedores
        List<Funko> funkos= await _context.Funkos
            .AsNoTracking()
            .OrderBy(f => f.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        Dictionary<string, Supplier> suppliersDict = await _context.Suppliers
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Id, ct); // a diccionario para mejores busquedas.
        // 2. Asignar el proveedor a cada funko de forma directa
        foreach (var funko in funkos)
        {
            if (suppliersDict.TryGetValue(funko.SupplierId, out var supplier))
            {
                funko.Supplier = supplier;
            }
        }
        // 3. Calcular metadatos
        int count = funkos.Count;
        int totalPages = (int)Math.Ceiling(
            (double)count / pageSize);
        bool hasPreviousPage = pageNumber > 1;
        bool hasNextPage = pageNumber < totalPages;

        return (
            funkos,
            count,
            totalPages,
            hasPreviousPage,
            hasNextPage
        );
    }

    // =========================================================
    // UPDATE
    // =========================================================

    /// <inheritdoc cref="IFunkoRepository.UpdateFunkoAsync(string, FunkoUpdateRequestDto, CancellationToken)"/>
    public async Task<Funko?> UpdateFunkoAsync(
        string id,
        FunkoUpdateRequestDto funko,
        CancellationToken ct = default)
    {
        var response = await _context.Funkos
            .Include(f => f.Supplier)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (response is null)
            return null;

        response.Name = funko.Name;
        response.Franchise = funko.Franchise;
        response.Number = funko.Number;
        response.Exclusive = funko.Exclusive;
        response.Price = funko.Price;
        response.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Entry(response)
                .Property(f => f.RowVersion)
                .OriginalValue = funko.RowVersion;
            await _context.SaveChangesAsync(ct);
        } catch(DbUpdateConcurrencyException ex)
        {
            Log.Error("[REPOSITORY-UPDATE] Error de concurrencia, al actualizar el funko con id {id}", id);
            throw ex;
        }

        return response;
    }
    /// <inheritdoc cref="IFunkoRepository.UpdateCategoryFromFunkoAsync(string, CategoryUpdateRequestDto, CancellationToken)"/>
    public async Task<Funko?> UpdateCategoryFromFunkoAsync(
        string id,
        CategoryUpdateRequestDto request,
        CancellationToken ct = default
    )
    {
        var response = await _context.Funkos
            .Include(f => f.Supplier)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (response is null)
            return null;
        response.Category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };
        await _context.SaveChangesAsync(ct);

        return response;
    }
    /// <inheritdoc cref="IFunkoRepository.UpdateSuplierFromFunkoAsync(string, string, CancellationToken)"/>
    public async Task<Funko?> UpdateSuplierFromFunkoAsync(
        string id,
        string SupplierId,
        CancellationToken ct = default
    )
    {
        var response = await _context.Funkos
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (response is null)
            return null;
        response.SupplierId = SupplierId;
        await _context.SaveChangesAsync(ct);
        await _context.Entry(response) // Obtener el proveedor al que hace referencia
            .Reference(f => f.Supplier)
            .LoadAsync(ct);

        return response;
    }

    
    // =========================================================
    // DELETE / RESTORE
    // =========================================================

    /// <inheritdoc cref="IFunkoRepository.DeleteFunkoAsync(string, CancellationToken)"/>
    public async Task<bool> DeleteFunkoAsync(
        string id,
        CancellationToken ct = default)
    {
        var response = await _context.Funkos
            .Include(f => f.Supplier)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (response is null)
            return false;

        response.IsDelete = true;
        response.DeleteAT = DateTime.UtcNow;
        response.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return true;
    }

    /// <inheritdoc cref="IFunkoRepository.RestoreFunkoAsync(string, CancellationToken)"/>
    public async Task<Funko?> RestoreFunkoAsync(
        string id,
        CancellationToken ct = default)
    {
        var response = await _context.Funkos
            .IgnoreQueryFilters()
            .Include(f => f.Supplier)
            .FirstOrDefaultAsync(f => f.Id == id && f.IsDelete, ct);

        if (response is null)
            return null;

        response.IsDelete = false;
        response.DeleteAT = null;
        response.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return response;
    }

    // =========================================================
    // Auxiliares
    // =========================================================

    /// <inheritdoc cref="IFunkoRepository.GetAllCategoriesAsync(CancellationToken)"/>
    public async Task<List<Category>> GetAllCategoriesAsync(
        CancellationToken ct = default)
    {
        var categories = await _context.Funkos
            .AsNoTracking()
            .Select(f => f.Category)
            .ToListAsync(ct);

        return categories
            .DistinctBy(c => c.Name)
            .ToList();
    }
    /// <inheritdoc cref="IFunkoRepository.ExistSupplierByIdAsync(int, CancellationToken)"/>
    public async Task<bool> ExistSupplierByIdAsync(
        string id,
        CancellationToken ct = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct) is not null;
    }
}
