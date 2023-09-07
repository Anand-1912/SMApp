using CQRS.Core.Commands;

namespace Post.Query.Api.Commands;
public class DeletePostCommand : BaseCommand
{
    public string UserName { get; set; } = null!;

}

