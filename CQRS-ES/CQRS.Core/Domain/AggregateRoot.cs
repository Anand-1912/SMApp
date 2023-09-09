using CQRS.Core.Events;
using System.Reflection.Metadata;

namespace CQRS.Core.Domain;
public abstract class AggregateRoot
{
    protected Guid _id;

    // events are used to make state changes to the aggregate
    private readonly List<BaseEvent> _changes = new();

    public Guid Id
    {
        get { return _id; }
    }

    // version is zero based
    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommitedChanges()
    {
        return _changes;
    }
   
    public void MarkChangesAsCommitted()
    {
        // this will be called after committing the uncommitted changes in eventstore
        _changes.Clear();
    }

    private void ApplyChange(BaseEvent @event, bool isNew)
    {
        // gets a method named 'Apply' and with paramters of type of @event (concrete type not BaseEvent)
        var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType()});

        if(method is null)
        {
            throw new ArgumentNullException(nameof(method),$"The Apply method is not available in the Aggregate for {@event.GetType().Name}!" );
        }

        method.Invoke(this, new object[] { @event });

        if (isNew)
        {
            _changes.Add( @event );
        }
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    // replay all the events for a given instance of an Aggregate from eventstore
    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach(var @event in events)
        {
            ApplyChange( @event, false);
        }
    }
}
