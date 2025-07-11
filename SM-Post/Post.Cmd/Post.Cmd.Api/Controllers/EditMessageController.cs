using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher) : EnhancedControllerBase
{
    [HttpPut("{id}")]
    public async Task<ActionResult> EditMessageAsync(Guid id, EditPostCommand command)
    {
        return await Do(logger, async () =>
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);
            return Ok(new BaseResponse { Message = "Message edited successfully" });
        });
    }
}