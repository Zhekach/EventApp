using EventApp.Models;
using EventApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController(IEventService eventService) : ControllerBase
{
    private readonly IEventService _eventService = eventService;

    /// <summary>
    /// Возвращает все доступные события
    /// </summary>
    /// <response code="200">Список событий. Если событий нет, возвращается пустой массив</response>
    [ProducesResponseType(typeof(IReadOnlyCollection<EventResponse>), StatusCodes.Status200OK)]
    [HttpGet]
    public ActionResult<IReadOnlyCollection<EventResponse>> GetAll()
    {
        return Ok(_eventService.GetAll());
    }

    /// <summary>
    /// Возвращает конкретное событие по его id, если такое событие существует
    /// </summary>
    /// <param name="id">Id для поиска события в формате Guid</param>
    /// <response code="200">Возвращает событие с требуемым id</response>
    /// <response code="400">Некорректный формат id</response>
    /// <response code="404">Событие с таким id не найдено</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public ActionResult<EventResponse> GetById([FromRoute] Guid id)
    {
        if (_eventService.TryGetById(id, out var result) == false)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Создаёт новое событие по переданным данным
    /// </summary>
    /// <param name="newEvent">Модель данных события</param>
    /// <response code="201">Создано новое событие</response>
    /// <response code="400">Некорректные данные</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] EventRequest newEvent)
    {
        if (_eventService.TryAdd(newEvent))
            return Created();
        
        return BadRequest();
    }

    /// <summary>
    /// Обновляет событие с переданным id по переданным данным, если такое событие есть в базе
    /// </summary>
    /// <param name="id">id события</param>
    /// <param name="updateEvent">Модель данных для обновления события</param>
    /// <response code="200">Событие обновлено</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">События с таким id не найдено</response>
    [HttpPut("{id}")]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update([FromRoute] Guid id, [FromBody] EventRequest updateEvent)
    {
        if(_eventService.TryUpdate(id, updateEvent))
            return Ok();
        
        return NotFound();
    }
    /// <summary>
    /// Удаляет событие с переданным id по переданным данным, если такое событие есть в базе
    /// </summary>
    /// <param name="id">id события</param>
    /// <response code="200">Событие удалено</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">События с таким id не найдено</response>
    [HttpDelete("{id}")]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] Guid id)
    {
        if(_eventService.TryDelete(id))
            return Ok();
        
        return NotFound();
    }
}
