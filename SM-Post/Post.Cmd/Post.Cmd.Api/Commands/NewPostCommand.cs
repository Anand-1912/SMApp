namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;
public class NewPostCommand: BaseCommand
{
    public string Author { get; set; } = null!;
    public string Message { get; set; } = null!;

}

