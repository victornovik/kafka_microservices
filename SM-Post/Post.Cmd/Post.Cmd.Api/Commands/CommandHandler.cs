﻿using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands;

public class CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler
{
    public async Task HandleAsync(AddCommentCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.AddComment(command.Comment, command.Username);
        await eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId, command.Comment, command.Username);
        await eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId, command.Username);
        await eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(NewPostCommand command)
    {
        var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
        await eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditPostCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditMessage(command.Message);
        await eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.LikePost();
        await eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.Username);
        await eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RestoreReadDbCommand command)
    {
        await eventSourcingHandler.RepublishEventsAsync();
    }
}