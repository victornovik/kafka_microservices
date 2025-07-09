using Post.Common.Events;

namespace Post.Query.Infra.Handlers;

public interface IEventHandler
{
    Task On(CommentAddedEvent evt);
    Task On(CommentRemovedEvent evt);
    Task On(CommentUpdatedEvent evt);
    Task On(MessageUpdatedEvent evt);
    Task On(PostCreatedEvent evt);
    Task On(PostLikedEvent evt);
    Task On(PostRemovedEvent evt);
}