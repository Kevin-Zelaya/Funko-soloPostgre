using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/suppliers")]
public class SupplierController(
    ISupplierService _service
) : ControllerBase
{
    // =========================================================
    // READ
    // =========================================================

    /// <summary>
    /// Obtiene todos los proveedores.
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult> GetAllSuppliers(
        CancellationToken ct)
    {
        var response = await _service.GetAllSuppliersAsync(ct);

        return response.IsSuccess
            ? Ok(response.Value)
            : BadRequest(response.Error);
    }

    /// <summary>
    /// Obtiene proveedores paginados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetPaginatedSuppliers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(
                "La página y el tamaño deben ser mayores que 0.");

        var response = await _service.GetPaginatedSuppliersAsync(
            pageNumber, pageSize, ct);

        return response.IsSuccess
            ? Ok(response.Value)
            : BadRequest(response.Error);
    }

    /// <summary>
    /// Obtiene un proveedor por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetSupplierById(
        string id,
        CancellationToken ct)
    {
        var response = await _service.GetSupplierByIdAsync(id, ct);

        return response.IsSuccess
            ? Ok(response.Value)
            : NotFound(response.Error);
    }
}