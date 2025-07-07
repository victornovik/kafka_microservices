using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private readonly Dictionary<Guid, Tuple<string, string>> comments = [];
    private string Author { get; set; }
    public bool IsActive { get; set; }

    public PostAggregate(){}

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent { Id = id, Author = author, Message = message, DatePosted = DateTime.UtcNow });
    }

    public void Apply(PostCreatedEvent evt)
    {
        AggregateId = evt.Id;
        IsActive = true;
        Author = evt.Author;
    }

    public void EditMessage(string message)
    {
        if (!IsActive)
            throw new InvalidOperationException("You cannot edit the message of inactive post");
        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty");

        RaiseEvent(new MessageUpdatedEvent { Id = AggregateId, Message = message });
    }

    public void Apply(MessageUpdatedEvent evt) => AggregateId = evt.Id;

    public void LikePost()
    {
        if (!IsActive)
            throw new InvalidOperationException("You cannot like an inactive post");

        RaiseEvent(new PostLikedEvent { Id = AggregateId });
    }

    public void Apply(PostLikedEvent evt) => AggregateId = evt.Id;

    public void AddComment(string comment, string username)
    {
        if (!IsActive)
            throw new InvalidOperationException("You cannot add comments to an inactive post");
        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty");

        RaiseEvent(new CommentAddedEvent { Id = AggregateId, CommentId = Guid.NewGuid(), Comment = comment, Username = username, CommentDate = DateTime.UtcNow });
    }

    public void Apply(CommentAddedEvent evt)
    {
        AggregateId = evt.Id;
        comments.Add(evt.CommentId, new Tuple<string, string>(evt.Comment, evt.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!IsActive)
            throw new InvalidOperationException("You cannot edit comment of inactive post");
        if (!comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to edit a comment of another user");

        RaiseEvent(new CommentUpdatedEvent { Id = AggregateId, CommentId = commentId, Comment = comment, Username = username, EditDate = DateTime.Now });
    }

    public void Apply(CommentUpdatedEvent evt)
    {
        AggregateId = evt.Id;
        comments[evt.CommentId] = new Tuple<string, string>(evt.Comment, evt.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!IsActive)
            throw new InvalidOperationException("You cannot remove a comment from inactive post");
        if (!comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to remove a comment of another user");

        RaiseEvent(new CommentRemovedEvent { Id = AggregateId, CommentId = commentId });
    }

    public void Apply(CommentRemovedEvent evt)
    {
        AggregateId = evt.Id;
        comments.Remove(evt.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!IsActive)
            throw new InvalidOperationException("Post is already removed");
        if (!Author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to delete a post of another user");

        RaiseEvent(new PostRemovedEvent { Id = AggregateId });
    }

    public void Apply(PostRemovedEvent evt)
    {
        AggregateId = evt.Id;
        IsActive = false;
    }
}