using EventApp.Models;

namespace EventApp.Services;

public class InMemoryEventService : IEventService
{
    private readonly Dictionary<Guid, Event> _events = new ();

    // //TODO Refactor Временная заглушка наполнения
    // private static Dictionary<Guid, Event> CreateInitialEvents()
    // {
    //     var model = Event.Create("Event1", DateTime.Today, DateTime.Today.AddDays(1), "dff");
    //     var result = new Dictionary<Guid, Event>();
    //     result.Add(model.Id, model);
    //
    //     return result;
    // }

    public PaginatedResult GetAll(EventsQuery query)
    {
        var filteredEvents = GetFilteredEvents(query);

        return GetPaginatedResult(filteredEvents, query);
    }

    public bool TryGetById(Guid id, out EventResponse? result)
    {
        if (_events.TryGetValue(id, out Event? model) == false)
        {
            result = null;
            return false;
        }

        result = MapModelToResponse(model);
        return true;
    }

    public bool TryAdd(EventRequest modelRequest)
    {
        var model = MapRequestToModel(modelRequest);

        if (_events.TryAdd(model.Id, model))
            return true;

        return false;
    }

    public bool TryUpdate(Guid id, EventRequest modelRequest)
    {
        try
        {
            if (TryGetById(id, out var resultResponse) && resultResponse != null)
            {
                var newModel = MapRequestToModel(modelRequest);
                _events[id] = _events[id].Update(newModel);
            }
            else
                return false;
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public bool TryDelete(Guid id)
    {
        if (_events.Remove(id))
            return true;

        return false;
    }

    private IEnumerable<Event> GetFilteredEvents(EventsQuery query)
    {
        IEnumerable<Event> filtered = _events.Values;

        if (string.IsNullOrWhiteSpace(query.Title) == false)
        {
            filtered = filtered.Where(e => e.Title.Contains(query.Title, StringComparison.InvariantCultureIgnoreCase));
        }

        if (query.From is not null)
        {
            filtered = filtered.Where(e => e.StartAt >= query.From);
        }

        if (query.To is not null)
        {
            filtered = filtered.Where(e => e.EndAt <= query.To);
        }

        return filtered;
    }

    private PaginatedResult GetPaginatedResult(IEnumerable<Event> events, EventsQuery query)
    {
        var totalItemsCount = events.Count();
        long itemsSkipCount = ((long)query.Page - 1) * query.PageSize;
        
        if (itemsSkipCount >= totalItemsCount)
        {
            var emptyResult = new PaginatedResult(totalItemsCount, new List<EventResponse>(), query.Page, 0);
        
            return emptyResult;
        }
        
        var items = events
            .Skip((int)itemsSkipCount)
            .Take(query.PageSize);
        
        var resultItems = new List<EventResponse>();
        foreach (var item in items)
        {
            resultItems.Add(MapModelToResponse(item));
        }
        
        var result = new PaginatedResult(totalItemsCount, resultItems, query.Page, resultItems.Count);
        
        return result;
    }

    private Event MapRequestToModel(EventRequest modelRequest)
    {
        var model = Event.Create(
            modelRequest.Title,
            modelRequest.StartAt,
            modelRequest.EndAt,
            modelRequest.Description);

        return model;
    }

    private EventResponse MapModelToResponse(Event model)
    {
        return new EventResponse(
            model.Id,
            model.Title,
            model.StartAt,
            model.EndAt,
            model.Description);
    }
}