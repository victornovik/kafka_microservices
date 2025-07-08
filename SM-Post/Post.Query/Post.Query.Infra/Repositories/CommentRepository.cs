using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Infra.DataAccess;

namespace Post.Query.Infra.Repositories;

public class CommentRepository(DatabaseContextFactory contextFactory) : ICommentRepository
{
    public async Task CreateAsync(CommentEntity comment)
    {
        using var context = contextFactory.CreateDbContext();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using var context = contextFactory.CreateDbContext();
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        using var context = contextFactory.CreateDbContext();
        var comment = await GetByIdAsync(commentId);
        if (comment == null)
            return;

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task<CommentEntity?> GetByIdAsync(Guid commentId)
    {
        using var context = contextFactory.CreateDbContext();
        return await context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
    }
}