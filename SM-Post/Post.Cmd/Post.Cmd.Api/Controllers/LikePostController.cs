using CQRS.Core.Exceptions;
using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LikePostController(ILogger<LikePostController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
{
    [HttpPut("{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id)
    {
        try
        {
            await commandDispatcher.SendAsync(new LikePostCommand { Id = id });
            return Ok(new BaseResponse { Message = "Like post request completed successfully" });
        }
        catch (InvalidOperationException ioe)
        {
            logger.Log(LogLevel.Warning, ioe, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ioe.Message });
        }
        catch (AggregateNotFoundException anfe)
        {
            logger.Log(LogLevel.Warning, anfe, "Could not retrieve aggregate, client passed an incorrect post ID");
            return BadRequest(new BaseResponse { Message = anfe.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to like a post";
            logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }
}