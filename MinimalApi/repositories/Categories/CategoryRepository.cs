using Microsoft.EntityFrameworkCore;
using Serilog;

public class CategoryPostgreRepository(
    AppDbContext _context
) : ICategoryRepository, IScoped
{
    /// <inheritdoc cref="ICategoryRepository.GetAllCategoriesAsync(CancellationToken)"/>
    public async Task<List<Category>> GetAllCategoriesAsync(
        CancellationToken ct
    )
    {
        return await _context.Categories
                    .AsNoTracking()
                    .ToListAsync(ct);
    }
    /// <inheritdoc cref="ICategoryRepository.GetPaginatedCategoriesAsync(int, int, CancellationToken)"/>
    public async Task<
        (
            List<Category> items,
            int count,
            int totalPages,
            bool HasPreviousPage,
            bool HasNextPage
        )
    > GetPaginatedCategoriesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        
        var response =  await _context.Categories
                    .AsNoTracking()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

        int count = response.Count();

        int totalPages = (int)Math.Ceiling(
            (double)count / pageSize);

        bool hasPreviousPage = pageNumber > 1;
        bool hasNextPage = pageNumber < totalPages;
        return (
            response,
            count,
            totalPages,
            hasPreviousPage,
            hasNextPage
        );
    }
    /// <inheritdoc cref="ICategoryRepository.GetCategoryByName(string, CancellationToken)"/>
    public async Task<Category?> GetCategoryByName(
        string name,
        CancellationToken ct
    )
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name, ct);
    }
    /// <inheritdoc cref="ICategoryRepository.CreateCategoryAsync(CategoryCreateRequestDto, CancellationToken)"/>
    public async Task<Category> CreateCategoryAsync(
        CategoryCreateRequestDto request, 
        CancellationToken ct)
    {
        var entidad = request.ToEntity();
        _context.Categories.Add(entidad);
        await _context.SaveChangesAsync(ct);
        return entidad;
    }
    /// <inheritdoc cref="ICategoryRepository.DeleteCategoryAsync(Guid, CancellationToken)"/>
    public async Task<bool> DeleteCategoryAsync(
        Guid id,
        CancellationToken ct)
    {
        var entidad = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        entidad.IsDelete = true;
        entidad.DeleteAT = DateTime.Now;
        await _context.SaveChangesAsync(ct);
        return true;
    }
    /// <inheritdoc cref="ICategoryRepository.RestoreCategoryAsync(Guid, CancellationToken)"/>
    public async Task<bool> RestoreCategoryAsync(
        Guid id,
        CancellationToken ct)
    {
        var entidad = await _context.Categories
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);
        entidad.IsDelete = false;
        await _context.SaveChangesAsync(ct);
        return true;
    }
    /// <inheritdoc cref="ICategoryRepository.UpdateCategoryAsync(Guid, CategoryUpdateRequestDto, CancellationToken)"/>
    public async Task<Category> UpdateCategoryAsync(
        Guid id,
        CategoryUpdateRequestDto request,
        CancellationToken ct
    )
    {
        var response = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if(response is null) return null;
        response.Name = request.Name;
        response.Description = request.Description;
        response.UpdatedAt = DateTime.Now;
        try
        {
            _context.Entry(response)
                .Property(f => f.RowVersion)
                .OriginalValue = request.RowVersion;
            await _context.SaveChangesAsync(ct);
        } catch(DbUpdateConcurrencyException ex)
        {
            Log.Error("[REPOSITORY-UPDATE] Error de concurrencia, al actualizar el funko con id {id}", id);
            throw ex;
        }
        return response;
    }
}
