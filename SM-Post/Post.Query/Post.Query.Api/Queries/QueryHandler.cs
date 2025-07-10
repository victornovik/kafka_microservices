using Post.Query.Domain.Entities;
using Post.Query.Infra.Repositories;

namespace Post.Query.Api.Queries;

public class QueryHandler(IPostRepository postRepository) : IQueryHandler
{
    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
    {
        return await postRepository.GetAllAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
    {
        return [await postRepository.GetByIdAsync(query.Id)];
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
    {
        return await postRepository.GetByAuthorAsync(query.Author);
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
    {
        return await postRepository.GetWithCommentsAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
    {
        return await postRepository.GetWithLikesAsync(query.Likes);
    }
}