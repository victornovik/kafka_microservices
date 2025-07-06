using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private readonly Dictionary<Guid, Tuple<string, string>> comments = [];
    private string Author { get; set; }
    public bool Active { get; set; }

    public PostAggregate(){}

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        Id = @event.Id;
        Active = true;
        Author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!Active)
            throw new InvalidOperationException("You cannot edit the message of inactive post");
        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty");

        RaiseEvent(new MessageUpdatedEvent
        {
            Id = this.Id,
            Message = message
        });
    }

    public void Apply(MessageUpdatedEvent @event) => Id = @event.Id;

    public void LikePost()
    {
        if (!Active)
            throw new InvalidOperationException("You cannot like an inactive post");

        RaiseEvent(new PostLikedEvent
        {
            Id = this.Id
        });
    }

    public void Apply(PostLikedEvent @event) => Id = @event.Id;

    public void AddComment(string comment, string username)
    {
        if (!Active)
            throw new InvalidOperationException("You cannot add comments to an inactive post");
        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty");

        RaiseEvent(new CommentAddedEvent
        {
            Id = this.Id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.UtcNow
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        Id = @event.Id;
        comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!Active)
            throw new InvalidOperationException("You cannot edit comment of inactive post");
        if (!comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to edit a comment of another user");

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = this.Id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.Now
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        Id = @event.Id;
        comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!Active)
            throw new InvalidOperationException("You cannot remove a comment from inactive post");
        if (!comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to remove a comment of another user");

        RaiseEvent(new CommentRemovedEvent
        {
            Id = this.Id,
            CommentId = commentId
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        Id = @event.Id;
        comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!Active)
            throw new InvalidOperationException("Post is already removed");
        if (!Author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to delete a post of another user");

        RaiseEvent(new PostRemovedEvent
        {
            Id = this.Id
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        Id = @event.Id;
        Active = false;
    }
}