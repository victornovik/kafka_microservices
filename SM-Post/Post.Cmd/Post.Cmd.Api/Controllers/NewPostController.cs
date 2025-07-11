using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher) : EnhancedControllerBase
{
    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
        return await Do(logger, async () =>
        {
            command.Id = Guid.NewGuid();
            await commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse { Id = command.Id, Message = "New post created successfully" });
        });
    }
}