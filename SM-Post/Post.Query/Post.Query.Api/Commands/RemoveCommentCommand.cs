﻿using CQRS.Core.Commands;

namespace Post.Query.Api.Commands;
public class RemoveCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string UserName { get; set; } = null!;

}
