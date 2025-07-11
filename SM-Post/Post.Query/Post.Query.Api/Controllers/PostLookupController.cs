using CQRS.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Post.Common.Dtos;
using Post.Query.Api.Dtos;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> dispatcher) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetAllPostsAsync()
        {
            try
            {
                var posts = await dispatcher.SendAsync(new FindAllPostsQuery());
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "Error while processing request to retrieve all posts");
            }
        }

        [HttpGet("byId/{postId}")]
        public async Task<ActionResult> GetByPostIdAsync(Guid postId)
        {
            try
            {
                var posts = await dispatcher.SendAsync(new FindPostByIdQuery { Id = postId });
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "Error while processing request to find post by ID");
            }
        }

        [HttpGet("byAuthor/{author}")]
        public async Task<ActionResult> GetPostsByAuthorAsync(string author)
        {
            try
            {
                var posts = await dispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author });
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "Error while processing request to find posts by author");
            }
        }

        [HttpGet("withComments")]
        public async Task<ActionResult> GetPostsWithCommentsAsync()
        {
            try
            {
                var posts = await dispatcher.SendAsync(new FindPostsWithCommentsQuery());
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "Error while processing request to find posts with comments");
            }
        }

        [HttpGet("withLikes/{numberOfLikes}")]
        public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
        {
            try
            {
                var posts = await dispatcher.SendAsync(new FindPostsWithLikesQuery { Likes = numberOfLikes });
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "Error while processing request to find posts with likes");
            }
        }

        private ActionResult NormalResponse(List<PostEntity> posts)
        {
            if (posts == null || !posts.Any())
                return NoContent();

            return Ok(new PostLookupResponse { Posts = posts, Message = $"Successfully returned {posts.Count} posts" });
        }

        private ActionResult ErrorResponse(Exception ex, string errorMessage)
        {
            logger.LogError(ex, errorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = errorMessage });
        }
    }
}