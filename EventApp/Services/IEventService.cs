using System.Diagnostics.Eventing.Reader;
using EventApp.Models;

namespace EventApp.Services;

public interface IEventService
{
    public PaginatedResult GetAll(EventsQuery query);
    public bool TryGetById(Guid id, out EventResponse? result);
    public bool TryAdd(EventRequest model);
    public bool TryUpdate(Guid id, EventRequest model);
    public bool TryDelete(Guid id);
}