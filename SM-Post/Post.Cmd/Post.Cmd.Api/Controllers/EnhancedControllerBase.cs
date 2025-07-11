using CQRS.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[Controller]
public abstract class EnhancedControllerBase : ControllerBase
{
    protected async Task<ActionResult> Do(ILogger logger, Func<Task<ActionResult>> func)
    {
        try
        {
            return await func();
        }
        catch (InvalidOperationException ioe)
        {
            logger.Log(LogLevel.Warning, ioe, "Client made bad request");
            return BadRequest(new BaseResponse { Message = ioe.Message });
        }
        catch (AggregateNotFoundException anfe)
        {
            logger.Log(LogLevel.Warning, anfe, "Could not retrieve aggregate, client passed incorrect post ID");
            return BadRequest(new BaseResponse { Message = anfe.Message });
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error while processing request");
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = "Error while processing request" });
        }
    }
}