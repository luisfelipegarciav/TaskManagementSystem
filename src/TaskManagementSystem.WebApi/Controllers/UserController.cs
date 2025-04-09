using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application;
using TaskManagementSystem.WebApi.Extensions;

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
        [Authorize(Roles = "admin")]
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

        [HttpPatch("/changepassword")]
        [Authorize]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(int), 401)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequestDto dto)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user");
            }
            var command = new ChangeUserPasswordCommand(userId.Value, dto);
            var result = await _mediator.Send(command);

            if (result.IsSuccessful)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message ?? "Unable to apply changes.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(int), 401)]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command);

            if (result.IsSuccessful)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message ?? "Unable to apply changes.");
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(int), 401)]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto updateUserDto)
        {
            var command = new UpdateUserCommand(updateUserDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccessful)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message ?? "Unable to apply changes.");
        }

        [HttpPost("/list")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(int), 401)]
        public async Task<IActionResult> GetUsersAsync([FromBody] PaginationParamsDto paginationParamsDto)
        {
            var command = new GetUsersCommand(paginationParamsDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccessful)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message ?? "Unable to pull users.");
        }
    }
}
