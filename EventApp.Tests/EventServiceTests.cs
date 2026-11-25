using EventApp.Models;
using EventApp.Services;

namespace EventApp.Tests;

public class EventServiceTests
{
    private readonly IEventService _eventService;
    
    public EventServiceTests()
    {
        _eventService = new InMemoryEventService();
    }
    
    [Fact]
    public void TryAdd_WithValidData_CreatesEvent()
    {
        //Arrange
        var newEventTitle = "Test Title";
        var newEventDescription = "Test Description";
        var newEventStartDate = new DateTime(2000, 1, 1);
        var newEventEndDate = new DateTime(2000, 1, 2);
        var newEvent = new EventRequest
        {
            Title = newEventTitle,
            Description = newEventDescription,
            StartAt = newEventStartDate,
            EndAt = newEventEndDate,
        };
        var eventsQuery = new EventsQuery(null, null, null,1,2);
        
        //Act
        var result = _eventService.TryAdd(newEvent);
        
        //Assert
        Assert.True(result);
        var addedEvent = Assert.Single(_eventService.GetAll(eventsQuery).CurrentEvents);
        Assert.Equal(newEventTitle, addedEvent.Title);
        Assert.Equal(newEventDescription, addedEvent.Description);
        Assert.Equal(newEventStartDate, addedEvent.StartAt);
        Assert.Equal(newEventEndDate, addedEvent.EndAt);
    }

    [Fact]
    public void GetAll_WithExistingEvents_ReturnsAllEvents()
    {
        //Arrange
        var newEventTitle1 = "Title1";
        var newEventTitle2 = "Title2";
        var newEventTitle3 = "Title3";
        var newEventStartDate = new DateTime(2000, 1, 1);
        var newEventEndDate = new DateTime(2000, 1, 2);
        
        var newEvent1 = new EventRequest
        {
            Title = newEventTitle1,
            StartAt = newEventStartDate,
            EndAt = newEventEndDate,
        };
        var newEvent2 = new EventRequest
        {
            Title = newEventTitle2,
            StartAt = newEventStartDate,
            EndAt = newEventEndDate,
        };
        var newEvent3 = new EventRequest
        {
            Title = newEventTitle3,
            StartAt = newEventStartDate,
            EndAt = newEventEndDate,
        };
        var eventsQueryPage = 1;
        var eventsQueryPageSize = 3;
        var eventsQuery = new EventsQuery(null, null, null,eventsQueryPage,eventsQueryPageSize);
        
        //Act
        var result = _eventService.TryAdd(newEvent1);
        result &= _eventService.TryAdd(newEvent2);
        result &= _eventService.TryAdd(newEvent3);
        var paginatedEvents = _eventService.GetAll(eventsQuery);
        
        //Assert
        Assert.True(result);
        Assert.Equal(3, paginatedEvents.TotalEventsCount);
        Assert.Equal(3, paginatedEvents.CurrentEvents.Count);
        Assert.Equal(3, paginatedEvents.CurrentPageEventsCount);
        Assert.Equal(eventsQueryPage, paginatedEvents.CurrentPageNumber);
        Assert.Contains(paginatedEvents.CurrentEvents, e => e.Title == newEventTitle1);
        Assert.Contains(paginatedEvents.CurrentEvents, e => e.Title == newEventTitle2);
        Assert.Contains(paginatedEvents.CurrentEvents, e => e.Title == newEventTitle3);
    }
}