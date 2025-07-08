using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Infra.DataAccess;

namespace Post.Query.Infra.Repositories;

public class PostRepository(DatabaseContextFactory contextFactory) : IPostRepository
{
    public async Task CreateAsync(PostEntity post)
    {
        using var context = contextFactory.CreateDbContext();
        context.Posts.Add(post);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using var context = contextFactory.CreateDbContext();
        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using var context = contextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);
        if (post == null)
            return;

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<PostEntity?> GetByIdAsync(Guid postId)
    {
        using var context = contextFactory.CreateDbContext();
        return await context.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<PostEntity>> GetAllAsync()
    {
        using var context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PostEntity>> GetByAuthorAsync(string author)
    {
        using var context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .Where(p => p.Author == author)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> GetWithLikesAsync(int likes)
    {
        using var context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .Where(p => p.Likes >= likes)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> GetWithCommentsAsync()
    {
        using var context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .Where(p => p.Comments != null && p.Comments.Any())
            .ToListAsync();
    }
}