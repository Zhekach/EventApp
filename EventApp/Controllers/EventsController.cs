using EventApp.Models;
using EventApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Controllers;

//TODO добавить DTO Request/Response, логику обработки вынести в сервис
[ApiController]
[Route("[controller]")]
public class EventsController(IEventService eventService) : ControllerBase
{
    private readonly IEventService _eventService = eventService;

    [HttpGet]
    public ActionResult<IReadOnlyCollection<EventResponse>> GetAll()
    {
        return Ok(_eventService.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Event> GetById([FromRoute] Guid id)
    {
        if (_eventService.TryGetById(id, out var result) == false)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create([FromBody] EventRequest newEvent)
    {
        if (_eventService.TryAdd(newEvent))
            return Created();
        
        return BadRequest();
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] Guid id, [FromBody] EventRequest updateEvent)
    {
        if(_eventService.TryUpdate(id, updateEvent))
            return Ok();
        
        return NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        if(_eventService.TryDelete(id))
            return Ok();
        
        return NotFound();
    }
}
