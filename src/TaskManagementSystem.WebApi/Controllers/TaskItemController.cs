using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application;
using TaskManagementSystem.WebApi.Extensions;

namespace TaskManagementSystem.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TaskItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(int), 400)]
        public async Task<IActionResult> CreateTaskItemAsync([FromBody] CreateTaskItemDto dtoBody)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user");
            }
            var command = new CreateTaskItemCommand(userId.Value, dtoBody);
            var result = await _mediator.Send(command);
            if (result.IsSuccessful)
            {
                return Created(uri: (string?)null, value: new { id = result.Data.Id });
            }
            return BadRequest(error: new { message = result.Message });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(int), 400)]
        public async Task<IActionResult> GetTaskItemAsync(int id)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user");
            }
            var command = new GetTaskItemCommand(userId.Value, id);
            var result = await _mediator.Send(command);
            if (result.IsSuccessful)
            {
                return Ok(result.Data);
            }
            return BadRequest(error: new { message = result.Message });
        }
    }
}
