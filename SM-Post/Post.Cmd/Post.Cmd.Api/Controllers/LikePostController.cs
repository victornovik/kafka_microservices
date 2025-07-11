using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LikePostController(ILogger<LikePostController> logger, ICommandDispatcher commandDispatcher) : EnhancedControllerBase
{
    [HttpPut("{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id)
    {
        return await Do(logger, async () =>
        {
            await commandDispatcher.SendAsync(new LikePostCommand { Id = id });
            return Ok(new BaseResponse { Message = "Post is liked successfully" });
        });
    }
}