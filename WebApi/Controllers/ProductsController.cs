using Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ProductsController(IMediator mediator) : ControllerBase
    {
        /// <summary>Единый поиск по строке q (SKU/Barcode/Name) с ранжированием и пагинацией</summary>
        /// <param name="q">Поисковая строка</param>
        /// <param name="page">Номер страницы (>=1)</param>
        /// <param name="pageSize">Размер страницы (1..200)</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchLine([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var result = await mediator.Send(new SearchLineQuery(q, page, pageSize), ct);
            return Ok(result);
        }
        [HttpGet("products/{*slugPath}")]
        public async Task<IActionResult> GetByCategory(
            string slugPath,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 24,
            CancellationToken ct = default)
        {
            var result = await mediator.Send(new GetProductsByCategorySlugQuery(slugPath, page, pageSize), ct);
            return Ok(result);
        }
    }
}
