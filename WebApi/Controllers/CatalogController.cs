using Application.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public sealed class CatalogController(IMediator mediator) : ControllerBase
    {
        [HttpGet("categories")]
        public async Task<IActionResult> GetRootCategories()
        {
            var result = await mediator.Send(new GetRootCategoriesQuery());
            return Ok(result);
        }

        [HttpGet("categoriesdepth")]
        public async Task<IActionResult> GetCategoriesTree()
        {
            var result = await mediator.Send(new GetCategoriesTreeQuery());
            return Ok(result);
        }
    }
}
