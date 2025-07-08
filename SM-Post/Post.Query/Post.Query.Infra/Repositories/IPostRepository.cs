using Post.Query.Domain.Entities;

namespace Post.Query.Infra.Repositories;

public interface IPostRepository
{
    Task CreateAsync(PostEntity post);
    Task UpdateAsync(PostEntity post);
    Task DeleteAsync(Guid postId);

    Task<PostEntity?> GetByIdAsync(Guid postId);

    // All methods below can only be used in read-only scenarios as they use `NoTracking` mode
    Task<List<PostEntity>> GetAllAsync();
    Task<List<PostEntity>> GetByAuthorAsync(string author);
    Task<List<PostEntity>> GetWithLikesAsync(int likes);
    Task<List<PostEntity>> GetWithCommentsAsync();
}