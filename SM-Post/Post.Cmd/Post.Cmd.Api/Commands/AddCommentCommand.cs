using CQRS.Core.Commands;

namespace Post.Query.Api.Commands;
public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; } = null!;
    public string UserName { get; set; } = null!;

}

