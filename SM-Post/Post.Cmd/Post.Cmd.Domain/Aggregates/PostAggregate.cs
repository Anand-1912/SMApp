using CQRS.Core.Domain;
using CQRS.Core.Messages;
using Post.Common.Events;
using System.ComponentModel.Design;

namespace Post.Cmd.Domain.Aggregates;

//post aggregate represent an instance of a post
public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author =null!;
    private readonly Dictionary<Guid, Tuple<string,string>> _comments = new();

    public bool Active
    {
        get =>_active;  
        set => _active = value; 
    }
    public PostAggregate() { }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent()
        {
            Id = id, Author = author, Message = message, DatePosted = DateTime.Now
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _author = @event.Author;
        _active = true;
    }

    public void EditMessage(string message)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot edit the message of an inactive post!");
        }

        if(string.IsNullOrEmpty(message))
        {
            throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}");
        }

        RaiseEvent(new MessageUpdatedEvent
        {
            Id = _id,
            Message = message
        });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot like an inactive post!");
        }

        RaiseEvent(new PostLikedEvent
        {
            Id = _id,
        });
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    public void AddComment(string comment, string username )
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot add a comment an inactive post!");
        }

        if (string.IsNullOrEmpty(comment))
        {
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}");
        }

        RaiseEvent(new CommentAddedEvent()
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            UserName = username,
            CommentDate = DateTime.Now

        });

    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new(@event.Comment, @event.UserName));

    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot edit a comment of an inactive post!");
        }

        if (!_comments.ContainsKey(commentId))
        {
            throw new InvalidOperationException("You cannot edit a comment that doesn't exist");
        }

        if (!_comments[commentId].Item2.Equals(username,StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user");
        }
        if (string.IsNullOrEmpty(comment))
        {
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}");
        }

        RaiseEvent(new CommentUpdatedEvent()
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            UserName = username,
            EditDate = DateTime.Now

        });

    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
       
        _comments[@event.CommentId] = new(@event.Comment, @event.UserName);

    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot remove a comment of an inactive post!");
        }

        if (!_comments.ContainsKey(commentId))
        {
            throw new InvalidOperationException("You cannot remove a comment that doesn't exist");
        }

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to remove a comment that was made by another user");
        }

        RaiseEvent(new CommentRemovedEvent()
        {
            Id = _id,
            CommentId = commentId,
            RemoveDate = DateTime.Now

        });

    }

    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);

    }

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("The post has already been removed");
        }
        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You cannot delete a post which was created by another user");
        }

        RaiseEvent(new PostRemovedEvent() { Id = _id });
    }

    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}
