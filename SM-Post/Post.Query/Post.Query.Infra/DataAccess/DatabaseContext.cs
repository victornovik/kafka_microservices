using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infra.DataAccess;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
}