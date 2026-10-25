using Microsoft.EntityFrameworkCore;

public class CategoryPostgreRepository(
    AppDbContext _context
) : ICategoryRepository, IScoped
{
    /// <inheritdoc cref="ICategoryRepository.GetAllCategoriesAsync(CancellationToken)"/>
    public async Task<List<Category>> GetAllCategoriesAsync(
        CancellationToken ct
    )
    {
        return await _context.Funkos
                    .AsNoTracking()
                    .Select(f => f.Category)
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
        
        var response =  await _context.Funkos
                    .AsNoTracking()
                    .Select(f => f.Category)
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
        return await _context.Funkos
                        .AsNoTracking()
                        .Select(f => f.Category)
                        .FirstOrDefaultAsync(c => c.Name == name, ct);
    }
}
