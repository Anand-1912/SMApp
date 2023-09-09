using CQRS.Core.Commands;

namespace Post.Query.Api.Commands;

public class EditMessageCommand : BaseCommand
{
    public string Message { get; set; } = null!;
}

