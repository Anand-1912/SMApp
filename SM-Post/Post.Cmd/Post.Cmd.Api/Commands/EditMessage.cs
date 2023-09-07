using CQRS.Core.Commands;

namespace Post.Query.Api.Commands;

public class EditMessage : BaseCommand
{
    public string Message { get; set; } = null!;
}

