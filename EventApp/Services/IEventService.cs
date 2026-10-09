using EventApp.Models;

namespace EventApp.Services;

public interface IEventService
{
    public IReadOnlyCollection<Event> GetAll();
    public bool TryGetById(Guid id, out Event? result);
    public bool TryAdd(Event model);
    public bool TryUpdate(Event model);
    public bool TryDelete(Guid id);
}