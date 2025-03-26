using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application;

namespace TaskManagementSystem.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(int), 400)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryDto dtoBody)
        {
            var command = new CreateCategoryCommand(dtoBody);
            var result = await _mediator.Send(command);
            if (result.IsSuccessful)
            {
                return Created(uri: (string?)null, value: new { id = result.Data.Id });
            }
            return BadRequest(error: result.Message);
        }
    }
}
