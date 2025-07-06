using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class EditCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string NewComment { get; set; }
    public string Username { get; set; }
}