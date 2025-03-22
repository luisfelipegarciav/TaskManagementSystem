using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application;

namespace TaskManagementSystem.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(int), 401)]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateUserDto dto)
        {
            var command = new AuthenticateUserCommand(dto.Username, dto.Password);
            var result = await _mediator.Send(command);

            if (result.IsSuccessful)
            {
                return Ok(result.Data);
            }

            return Unauthorized("Invalid credentials.");
        }
    }
}
