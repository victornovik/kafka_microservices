using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AddCommentController(ILogger<AddCommentController> logger, ICommandDispatcher commandDispatcher) : EnhancedControllerBase
{
    [HttpPut("{id}")]
    public async Task<ActionResult> AddCommentAsync(Guid id, AddCommentCommand command)
    {
        return await Do(logger, async () =>
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);
            return Ok(new BaseResponse { Message = "Comment is added successfully" });
        });
    }
}