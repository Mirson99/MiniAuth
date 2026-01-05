using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniAuth.Application.Auth.Commands.Register;
using MiniAuth.Domain.Enums;
using MiniAuth.Infrastructure.Authorization;

namespace MiniAuth.API.Controllers;

[ApiController]
[Route("example")]
public class ExampleController: ControllerBase
{
    [Authorize]
    [HasPermission(Permission.ReadMember)]
    [HttpGet("")]
    public async Task<IActionResult> Example()
    {
        return Ok();
    }
    
    [HasPermission(Permission.WriteMember)]
    [HttpGet("write")]
    public async Task<IActionResult> WriteExample()
    {
        return Ok();
    }
}