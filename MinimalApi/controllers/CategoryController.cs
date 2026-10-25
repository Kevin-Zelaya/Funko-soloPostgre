using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/categories")]
public class CategoryController(
    ICategoryService _service
) : ControllerBase
{
    // =========================================================
    // READ
    // =========================================================

    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult> GetAllCategories(
        CancellationToken ct)
    {
        var response = await _service.GetAllCategoriesAsync(ct);

        return response.IsSuccess
            ? Ok(response.Value)
            : BadRequest(response.Error);
    }

    /// <summary>
    /// Obtiene las categorías paginadas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetPaginatedCategories(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(
                "La página y el tamaño deben ser mayores que 0.");

        var response = await _service.GetPaginatedCategoriesAsync(
            pageNumber, pageSize, ct);

        return response.IsSuccess
            ? Ok(response.Value)
            : BadRequest(response.Error);
    }

    /// <summary>
    /// Obtiene una categoría por su nombre.
    /// </summary>
    [HttpGet("{name}")]
    public async Task<ActionResult> GetCategoryByName(
        string name,
        CancellationToken ct)
    {
        var response = await _service.GetCategoryByNameAsync(name, ct);

        return response.IsSuccess
            ? Ok(response.Value)
            : NotFound(response.Error);
    }
}