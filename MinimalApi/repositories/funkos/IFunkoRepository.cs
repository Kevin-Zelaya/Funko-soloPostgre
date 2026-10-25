public interface IFunkoRepository
{
    /// <summary>
    /// Crear un funko de forma asincrona
    /// </summary>
    /// <param name="funko">Modelo funko con propiedades a crear</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Funko creado</returns>
    Task<Funko> CreateFunkoAsync(Funko funko, CancellationToken ct = default);
    /// <summary>
    /// Eliminar de forma lógica un funko
    /// se le agrega el tiempo de eliminación
    /// </summary>
    /// <param name="id">Id del funkko a eliminar</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>True o false dependiendo de si se encontró o no</returns>
    Task<bool> DeleteFunkoAsync(string id, CancellationToken ct);
    /// <summary>
    /// Obtener todos los funkos, incluyen su categoria
    /// no página
    /// </summary>
    /// <param name="ct">token de cancelación</param>
    /// <returns> Lista sin filtrar de funkos </returns>
    Task<List<Funko>> GetAllFunkosAsync(CancellationToken ct);
    /// <summary>
    /// Obtener un funko por id
    /// </summary>
    /// <param name="id">id del usuario a buscar</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Funko encontrado o null</returns>
    Task<Funko?> GetFunkoByIdAsync(string id, CancellationToken ct);
    /// <summary>
    /// Obtener lista páginada de funkos
    /// </summary>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="ct">token de cancelación</param>
    /// <returns>Lista paginada de usuarios</returns>
    Task<(List<Funko> items, int count, int totalPages, bool HasPreviousPage, bool HasNextPage)> GetPaginatedFunkosAsync(int pageNumber, int pageSize, CancellationToken ct);
    /// <summary>
    /// Restablecer funko eliminado de forma lógica
    /// </summary>
    /// <param name="id">Id del funko a restablecer</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>True o false dependiendo de si se encontró</returns>
    Task<Funko?> RestoreFunkoAsync(string id, CancellationToken ct);
    /// <summary>
    /// Actualizar funko
    /// </summary>
    /// <param name="id">Id del funko a actualizar</param>
    /// <param name="funko">Data del funko</param>
    /// <param name="ct">Token a cancelación</param>
    /// <returns>Data actualiada</returns>
    Task<Funko?> UpdateFunkoAsync(string id, FunkoUpdateRequestDto funko, CancellationToken ct = default);
    /// <summary>
    /// Auxiliar para obtener todas las categorias
    /// </summary>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Lista de categorias</returns>
    public Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct);
    /// <summary>
    /// Actualización parcial, solo
    /// categoria del funko
    /// </summary>
    /// <param name="id">Id del funko a actualizar</param>
    /// <param name="request">Dto de categoría a actualizar</param>
    /// <see cref="CategoryUpdateRequestDto"/>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Funko actualizado</returns>
    public  Task<Funko?> UpdateCategoryFromFunkoAsync(string id, CategoryUpdateRequestDto request, CancellationToken ct = default);
    /// <summary>
    /// Actualizacion parcial, solo
    /// referencia a proveedor
    /// </summary>
    /// <param name="id">Id del funko a actualizar</param>
    /// <param name="SupplierId">Id del proveedor</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Funko actualizado</returns>
    public Task<Funko?> UpdateSuplierFromFunkoAsync(string id, string SupplierId, CancellationToken ct = default);
    /// <summary>
    /// Comprueba si existe el proveedor
    /// </summary>
    /// <param name="id">Id del proveedor a consultar</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Proveedor consultado</returns>
    public  Task<bool> ExistSupplierByIdAsync(string id, CancellationToken ct);
}