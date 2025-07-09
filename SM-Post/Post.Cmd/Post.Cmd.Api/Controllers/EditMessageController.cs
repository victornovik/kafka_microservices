using CQRS.Core.Exceptions;
using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
{
    [HttpPut("{id}")]
    public async Task<ActionResult> EditMessageAsync(Guid id, EditPostCommand command)
    {
        try
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);
            return Ok(new BaseResponse { Message = "Edit message request completed successfully" });
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
            const string SAFE_ERROR_MESSAGE = "Error while processing request to edit message";
            logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }
}