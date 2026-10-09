using EventApp.Models;

namespace EventApp.Services;

public class InMemoryEventService : IEventService
{
    private readonly Dictionary<Guid, Event> _events = CreateInitialEvents();

    //TODO Refactor Временная заглушка наполнения
    private static Dictionary<Guid, Event> CreateInitialEvents()
    {
        var model = Event.Create("Event1", DateTime.Now, DateTime.Today, "dff");
        var result = new Dictionary<Guid, Event>();
        result.Add(model.Id, model);

        return result;
    }

    public IReadOnlyCollection<EventResponse> GetAll()
    {
        var result = new List<EventResponse>();

        foreach (var model in _events.Values)
        {
            result.Add(MapModelToResponse(model));
        }

        return result;
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