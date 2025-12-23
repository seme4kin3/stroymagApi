using Application.Admin.Attributes.Commands;
using Application.Admin.Attributes.Queries;
using Application.Admin.Brands.Commands;
using Application.Admin.Brands.Queries;
using Application.Admin.Categories.Commands;
using Application.Admin.Categories.Queries;
using Application.Admin.MeasurementUnits;
using Application.Admin.MeasurementUnits.Commands;
using Application.Admin.MeasurementUnits.Queries;
using Application.Admin.Products.Commands;
using Application.Common;
using Infrastructure.Import;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public sealed class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
           _mediator = mediator;
        }

        [HttpGet("attributes")]
        public async Task<IActionResult> GetPagedAttribute([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetAttributesPagedQuery(page, pageSize), ct);
            return Ok(result);
        }

        [HttpPost("attributes")]
        public async Task<IActionResult> CreateAttribute(
            [FromBody] CreateAttributeCommand cmd,
            CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetByIdAttribute), new { id }, new { id });
        }

        // простой GET по id — чтобы CreatedAtAction не ругался (можно потом допилить)
        [HttpGet("attributes/{id:guid}")]
        public IActionResult GetByIdAttribute(Guid id)
            => Ok(new { id });


        [HttpPut("attributes/{id:guid}")]
        public async Task<IActionResult> UpdateAttribute(
            Guid id,
            [FromBody] UpdateAttributeCommand body,
            CancellationToken ct)
        {
            var cmd = body with { Id = id };
            await _mediator.Send(cmd, ct);
            return NoContent();
        }

        [HttpDelete("attributes/{id:guid}")]
        public async Task<IActionResult> DeleteAttribute(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteAttributeCommand(id), ct);
            return NoContent();
        }

        // 2) Создать категорию
        // Создание категории + атрибуты (обязательно)

        [HttpGet("categories")]
        public async Task<IActionResult> GetPagedCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetCategoriesPagedQuery(page, pageSize), ct);
            return Ok(result);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryCommand cmd,
            CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return Created(string.Empty, new { id });
        }

        [HttpGet("categories/{id:guid}")]
        public async Task<IActionResult> GetCategoryAdminById(Guid id, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetCategoryAdminByIdQuery(id), ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        // Обновление категории + полный пересбор атрибутов
        [HttpPut("categories/{id:guid}")]
        public async Task<IActionResult> UpdateCategory(
            Guid id,
            [FromBody] UpdateCategoryCommand body,
            CancellationToken ct)
        {
            var cmd = body with { Id = id };
            await _mediator.Send(cmd, ct);
            return NoContent();
        }

        // Удаление категории
        [HttpDelete("categories/{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteCategoryCommand(id), ct);
            return NoContent();
        }

        // 4) Создать товар (и применить значения атрибутов категории)

        [HttpGet("products")]
        public async Task<IActionResult> GetPagedProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetCategoriesPagedQuery(page, pageSize), ct);
            return Ok(result);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductCommand cmd,
            CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return Created(string.Empty, new { id });
        }

        [HttpPut("products/{id:guid}")]
        public async Task<IActionResult> UpdateProduct(
            Guid id,
            [FromBody] UpdateProductCommand body,
            CancellationToken ct)
        {
            var cmd = body with { Id = id };
            await _mediator.Send(cmd, ct);
            return NoContent();
        }

        [HttpDelete("products/{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteProductCommand(id), ct);
            return NoContent();
        }


        [HttpGet("brands")]
        public async Task<IActionResult> GetPagedBrands(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetBrandsPagedQuery(page, pageSize), ct);
            return Ok(result);
        }


        [HttpPost("brands")]
        public async Task<IActionResult> CreateBrand(
            [FromBody] CreateBrandCommand cmd,
            CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return Created(string.Empty, new { id });
        }


        [HttpPut("brands/{id:guid}")]
        public async Task<IActionResult> UpdateBrand(
            Guid id,
            [FromBody] UpdateBrandCommand body,
            CancellationToken ct)
        {
            var cmd = body with { Id = id };
            await _mediator.Send(cmd, ct);
            return NoContent();
        }


        [HttpDelete("brands/{id:guid}")]
        public async Task<IActionResult> DeleteBrand(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteBrandCommand(id), ct);
            return NoContent();
        }

        [HttpGet("units")]
        public async Task<ActionResult<PagedResult<MeasurementUnitListItemDto>>> GetPagedUnits(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
         => Ok(await _mediator.Send(new GetMeasurementUnitsPagedQuery(page, pageSize), ct));

        [HttpPost("units")]
        public async Task<IActionResult> CreateUnit(
            [FromBody] CreateMeasurementUnitCommand cmd,
            CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return Created(string.Empty, new { id });
        }

        [HttpPut("units/{id:guid}")]
        public async Task<IActionResult> UpdateUnit(
            Guid id,
            [FromBody] UpdateMeasurementUnitCommand body,
            CancellationToken ct)
        {
            await _mediator.Send(body with { Id = id }, ct);
            return NoContent();
        }

        [HttpDelete("units/{id:guid}")]
        public async Task<IActionResult> DeleteUnit(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteMeasurementUnitCommand(id), ct);
            return NoContent();
        }

        [HttpPost("excel")]
        public async Task<IActionResult> UploadExcel(IFormFile file, [FromServices] IExcelImportService import, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не передан");

            using var stream = file.OpenReadStream();
            var result = await import.ImportAsync(stream, ct);

            // Если остановились из-за пустого SKU — это «частичный успех», но не ошибка 500/400.
            return Ok(result);
        }
    }
}
