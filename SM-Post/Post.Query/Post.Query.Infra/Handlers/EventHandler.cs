using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Infra.Repositories;

namespace Post.Query.Infra.Handlers;

public class EventHandler(IPostRepository postRepository, ICommentRepository commentRepository) : IEventHandler
{
    public async Task On(CommentAddedEvent evt)
    {
        var comment = new CommentEntity
        {
            CommentId = evt.CommentId,
            Comment = evt.Comment,
            CommentDate = evt.CommentDate,
            IsEdited = false,
            PostId = evt.Id,
            UserName = evt.Username
        };
        await commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentRemovedEvent evt)
    {
        await commentRepository.DeleteAsync(evt.CommentId);
    }

    public async Task On(CommentUpdatedEvent evt)
    {
        var comment = await commentRepository.GetByIdAsync(evt.CommentId);
        if (comment == null)
            return;

        comment.Comment = evt.Comment;
        comment.IsEdited = true;
        comment.CommentDate = evt.EditDate;

        await commentRepository.UpdateAsync(comment);
    }

    public async Task On(MessageUpdatedEvent evt)
    {
        var post = await postRepository.GetByIdAsync(evt.Id);
        if (post == null)
            return;

        post.Message = evt.Message;
        await postRepository.UpdateAsync(post);
    }

    public async Task On(PostCreatedEvent evt)
    {
        var post = new PostEntity { PostId = evt.Id, Author = evt.Author, DatePosted = evt.DatePosted, Message = evt.Message };
        await postRepository.CreateAsync(post);
    }

    public async Task On(PostLikedEvent evt)
    {
        var post = await postRepository.GetByIdAsync(evt.Id);
        if (post == null)
            return;

        post.Likes++;
        await postRepository.UpdateAsync(post);
    }

    public async Task On(PostRemovedEvent evt)
    {
        await postRepository.DeleteAsync(evt.Id);
    }
}