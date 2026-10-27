
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/suppliers")]
public class SupplierController(
    ISupplierService _service
) : ControllerBase
{
    // =========================================================
    // CREATE
    // =========================================================

    /// <summary>
    /// Crea un nuevo proveedor.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateSupplier(
        [FromBody] SupplierCreateRequestDto request,
        CancellationToken ct)
    {
        var response = await _service.CreateSupplierAsync(request, ct);

        return response.IsSuccess
                ? CreatedAtAction(
                nameof(GetSupplierById),
                new { controller = "Supplier", id = response.Value.Id },
                response.Value)
            : BadRequest(response.Error);
    }

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

    // =========================================================
    // UPDATE
    // =========================================================

    /// <summary>
    /// Actualiza un proveedor existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSupplier(
        string id,
        [FromBody] SupplierUpdateRequestDto request,
        CancellationToken ct)
    {
        var response = await _service.UpdateSupplierAsync(id, request, ct);

        return response.IsSuccess
            ? Ok(response.Value)
            : NotFound(response.Error);
    }

    // =========================================================
    // DELETE
    // =========================================================

    /// <summary>
    /// Elimina lógicamente un proveedor.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSupplier(
        string id,
        CancellationToken ct)
    {
        var response = await _service.DeleteSupplierAsync(id, ct);

        return response.IsSuccess
            ? NoContent()
            : NotFound(response.Error);
    }

    // =========================================================
    // RESTORE
    // =========================================================

    /// <summary>
    /// Restaura un proveedor eliminado lógicamente.
    /// </summary>
    [HttpPatch("{id}/restore")]
    public async Task<ActionResult> RestoreSupplier(
        string id,
        CancellationToken ct)
    {
        var response = await _service.RestoreSupplierAsync(id, ct);

        return response.IsSuccess
            ? NoContent()
            : NotFound(response.Error);
    }
}