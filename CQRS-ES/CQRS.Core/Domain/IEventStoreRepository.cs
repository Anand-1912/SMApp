using CQRS.Core.Events;

namespace CQRS.Core.Domain;

// event store should be immutable.So, data can be only be persisted and retrieved
// but cannot be updated or deleted
public interface IEventStoreRepository
{
    Task SaveAsync(EventModel @event);
    Task<List<EventModel>> FindAggregateById(Guid aggregateId);

}
