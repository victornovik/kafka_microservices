using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher) : EnhancedControllerBase
{
    [HttpPost]
    public async Task<ActionResult> RestoreReadDbAsync()
    {
        return await Do(logger, async () =>
        {
            await commandDispatcher.SendAsync(new RestoreReadDbCommand());
            return Ok(new BaseResponse { Message = "Read database is restored successfully" });
        });
    }
}