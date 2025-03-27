using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application;

namespace TaskManagementSystem.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
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
            return BadRequest(error: new { message =  result.Message });
        }

        [HttpGet]
        [Authorize(Roles = "admin, taskuser")]
        [ProducesResponseType(typeof(List<CategoryDto>), 200)]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var command = new GetCategoryCommand();
            var result = await _mediator.Send(command);
            if (result.IsSuccessful)
            {
                return Ok(result.Data);
            }
            return BadRequest(error: new { message = result.Message ?? "Unable to pull categories." });
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(int), 400)]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryDto dtoBody)
        {
            var command = new UpdateCategoryCommand(id, dtoBody);
            var result = await _mediator.Send(command);
            if (result.IsSuccessful)
            {
                return Ok();
            }
            return BadRequest(error: new { message = result.Message });
        }
    }
}
