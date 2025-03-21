using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application;

namespace TaskManagementSystem.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(int), 400)]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto dtoBody)
        {
            var command = new CreateUserCommand(dtoBody);
            var result = await _mediator.Send(command);
            if (result.IsSuccessful)
            {
                return Created(uri: (string?)null, value: new { id = result.Data.Id });
            }
            return BadRequest(error: result.Message);
        }
    }
}
