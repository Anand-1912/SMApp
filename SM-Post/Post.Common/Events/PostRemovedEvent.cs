using CQRS.Core.Events;

namespace Post.Common.Events;
public class PosttRemovedEvent : BaseEvent
{
    public PosttRemovedEvent() : base(nameof(PosttRemovedEvent))
    {
    }

}
