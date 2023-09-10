using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

/* 

EventStore implementation contains business logic that is required to version events 
of a given aggregate and for persisting new events to EventStore Database by using the
Event Store Repository. It also ensures that Concurrent updates to EventStore are appropriately 
handled using Optimisic Concurrency Control 

*/
public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }
    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindAggregateById(aggregateId);
        if(eventStream == null || !eventStream.Any())
        {
            throw new AggregateNotFoundException("Incorrect Id Provided");
        }
        return eventStream.OrderBy(x=>x.Version).Select(x=>x.EventData).ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindAggregateById(aggregateId);

        // optimistic concurrency control
        if(expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
        {
            throw new ConcurrencyException();
        }

        var version = expectedVersion;
        foreach(var @event in events)
        {            
            await _eventStoreRepository.SaveAsync(new EventModel()
            {
                Id = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = ++version,
                EventType = @event.GetType().Name,
                EventData = @event
            });

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _eventProducer.ProduceAsync(topic = null!, @event);
        }

    }
}
