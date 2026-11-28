using EventApp.Models;
using EventApp.Services;

namespace EventApp.Tests;

public class EventServiceTests
{
    private readonly IEventService _eventService;
    private readonly Random _random;
    private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly EventsQuery _eventsQueryDefault;

    public EventServiceTests()
    {
        _eventService = new InMemoryEventService();
        _random = new Random();
        _eventsQueryDefault = new EventsQuery(null, null, null, 1, 10);
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

        //Act
        var isEventAdded = _eventService.TryAdd(newEvent);

        //Assert
        Assert.True(isEventAdded);
        var addedEvent = Assert.Single(_eventService.GetAll(_eventsQueryDefault).CurrentEvents);
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

        var newEvent1 = GenerateNewEventRequest(newEventTitle1);
        var newEvent2 = GenerateNewEventRequest(newEventTitle2);
        var newEvent3 = GenerateNewEventRequest(newEventTitle3);
        var eventsQueryPage = 1;
        var eventsQueryPageSize = 3;
        var eventsQuery = new EventsQuery(null, null, null, eventsQueryPage, eventsQueryPageSize);

        //Act
        var isEventsAdded = _eventService.TryAdd(newEvent1);
        isEventsAdded &= _eventService.TryAdd(newEvent2);
        isEventsAdded &= _eventService.TryAdd(newEvent3);
        var paginatedEvents = _eventService.GetAll(eventsQuery);

        //Assert
        Assert.True(isEventsAdded);
        Assert.Equal(3, paginatedEvents.TotalEventsCount);
        Assert.Equal(3, paginatedEvents.CurrentEvents.Count);
        Assert.Equal(3, paginatedEvents.CurrentPageEventsCount);
        Assert.Equal(eventsQueryPage, paginatedEvents.CurrentPageNumber);
        Assert.Contains(paginatedEvents.CurrentEvents, e => e.Title == newEventTitle1);
        Assert.Contains(paginatedEvents.CurrentEvents, e => e.Title == newEventTitle2);
        Assert.Contains(paginatedEvents.CurrentEvents, e => e.Title == newEventTitle3);
    }

    [Fact]
    public void GetById_WithExistingId_ReturnsEvent()
    {
        //Arrange
        var newEventTitle1 = "Title1";
        var newEventTitle2 = "Title2";
        var request1 = GenerateNewEventRequest(newEventTitle1);
        var request2 = GenerateNewEventRequest(newEventTitle2);

        //Act
        var iseventsAdded = _eventService.TryAdd(request1);
        iseventsAdded &= _eventService.TryAdd(request2);
        var paginatedEvents = _eventService.GetAll(_eventsQueryDefault);
        var eventsResponse = paginatedEvents.CurrentEvents;

        //Assert
        Assert.True(iseventsAdded);

        foreach (var expected in new[] { request1, request2 })
        {
            var expectedId = Assert.Single(eventsResponse, e => e.Title == expected.Title).Guid;
            var isEventFound = _eventService.TryGetById(expectedId, out var actualResponse);

            Assert.True(isEventFound);
            Assert.NotNull(actualResponse);
            Assert.Equal(expectedId, actualResponse.Guid);
            Assert.Equal(expected.Title, actualResponse.Title);
            Assert.Equal(expected.StartAt, actualResponse.StartAt);
            Assert.Equal(expected.EndAt, actualResponse.EndAt);
        }
    }

    [Fact]
    public void TryUpdate_WithExistingId_UpdatesEvent()
    {
        //Arrange
        var baseTitle = "Base title";
        var baseRequest = GenerateNewEventRequest(baseTitle);
        var updateEventRequest = baseRequest with
        {
            Title = "Update Title",
            Description = "Updated description",
            StartAt = baseRequest.StartAt.AddDays(3),
            EndAt = baseRequest.EndAt.AddDays(3),
        } ;

        //Act
        var isEventAdded = _eventService.TryAdd(baseRequest);
        var baseEventResponse = _eventService.GetAll(_eventsQueryDefault).CurrentEvents.
            First(e => e.Title == baseTitle);
        var eventId = baseEventResponse.Guid;
        var isUpdated = _eventService.TryUpdate(eventId, updateEventRequest);
        var isUpdateReceivedById = _eventService.TryGetById(eventId, out var updatedEventResponse);

        //Assert
        Assert.True(isEventAdded);
        Assert.True(isUpdated);
        Assert.True(isUpdateReceivedById);
        Assert.NotNull(updatedEventResponse);
        Assert.Equal(eventId, updatedEventResponse.Guid);
        Assert.Equal(updateEventRequest.Title, updatedEventResponse.Title);
        Assert.Equal(updateEventRequest.Description, updatedEventResponse.Description);
        Assert.Equal(updateEventRequest.StartAt, updatedEventResponse.StartAt);
        Assert.Equal(updateEventRequest.EndAt, updatedEventResponse.EndAt);
    }
    
    [Fact]
    public void TryDelete_WithExistingId_DeletesEvent()
    {
        //Arrange
        var titleToDelete = "Title to delete";
        var titleToKeep = "Title to keep";
        Assert.True(_eventService.TryAdd(GenerateNewEventRequest(titleToDelete)));
        Assert.True(_eventService.TryAdd(GenerateNewEventRequest(titleToKeep)));
        var events = _eventService.GetAll(_eventsQueryDefault).CurrentEvents;
        var eventToDeleteId = Assert.Single(events, e => e.Title == titleToDelete).Guid;
        var eventToKeepId = Assert.Single(events, e => e.Title == titleToKeep).Guid;

        //Act
        var isDeleted = _eventService.TryDelete(eventToDeleteId);

        //Assert
        Assert.True(isDeleted);
        Assert.False(_eventService.TryGetById(eventToDeleteId, out var deletedEventResponse));
        Assert.Null(deletedEventResponse);
        
        var eventToKeepResponse = Assert.Single(_eventService.GetAll(_eventsQueryDefault).CurrentEvents);
        Assert.Equal(eventToKeepId, eventToKeepResponse.Guid);
    }

    private EventRequest GenerateNewEventRequest(string title, string description = "description")
    {
        var startAt = new DateTime(2000, 1, 1);
        var endAt = new DateTime(2000, 1, 2);
        return new EventRequest
        {
            Title = title,
            StartAt = startAt,
            EndAt = endAt,
            Description = description,
        };
    }
    
    private EventRequest GenerateNewEventRequest(string title, DateTime startAt, DateTime endAt, 
        string description = "description")
    {
        return new EventRequest
        {
            Title = title,
            StartAt = startAt,
            EndAt = endAt,
            Description = description,
        };
    }

    private EventRequest GenerateNewEventRequest()
    {
        var randomTitle = _random.GetString(chars, 5);

        return GenerateNewEventRequest(randomTitle);
    }
}
