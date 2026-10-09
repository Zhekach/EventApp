using EventApp.Models;

namespace EventApp.Services;

public class InMemoryEventService : IEventService
{
    private readonly Dictionary<Guid, Event> _events = CreateInitialEvents();

    //TODO Refactor Временная заглушка
    private static Dictionary<Guid, Event> CreateInitialEvents()
    {
        var model = Event.Create("Event1", DateTime.Now, DateTime.Today, "dff");
        var result = new Dictionary<Guid, Event>();
        result.Add(model.Id, model);
        
        return result;
    }
    
    public IReadOnlyCollection<Event> GetAll()
    {
        return _events.Values;
    }

    public bool TryGetById(Guid id, out Event? result)
    {
        return _events.TryGetValue(id, out result);
    }

    public bool TryAdd(Event model)
    {
        if (_events.TryAdd(model.Id, model))
            return true;

        return false;
    }

    public bool TryUpdate(Event model)
    {
        try
        {
            if(TryGetById(model.Id, out var result) && result != null)
                _events[model.Id] = result.Update(model);
            else
                return  false;
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public bool TryDelete(Guid id)
    {
        if(_events.Remove(id))
            return true;
        
        return false;
    }
}