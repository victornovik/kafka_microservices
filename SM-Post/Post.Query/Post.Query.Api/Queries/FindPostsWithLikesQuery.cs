﻿using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;

public class FindPostsWithLikesQuery : BaseQuery
{
    public int Likes { get; set; }
}