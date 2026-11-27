using EventApp.Models;
using EventApp.Services;

namespace EventApp.Tests;

public class EventServiceTests
{
    private readonly IEventService _eventService;
    private readonly Random _random;
    private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public EventServiceTests()
    {
        _eventService = new InMemoryEventService();
        _random = new Random();
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
        var eventsQuery = new EventsQuery(null, null, null, 1, 2);

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

        var newEvent1 = GenerateNewEventRequest(newEventTitle1);
        var newEvent2 = GenerateNewEventRequest(newEventTitle2);
        var newEvent3 = GenerateNewEventRequest(newEventTitle3);
        var eventsQueryPage = 1;
        var eventsQueryPageSize = 3;
        var eventsQuery = new EventsQuery(null, null, null, eventsQueryPage, eventsQueryPageSize);

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

    [Fact]
    public void GetById_WithExistingId_ReturnsEvent()
    {
        //Arrange
        var newEventTitle1 = "Title1";
        var newEventTitle2 = "Title2";
        var request1 = GenerateNewEventRequest(newEventTitle1);
        var request2 = GenerateNewEventRequest(newEventTitle2);
        var startAt = request1.StartAt;
        var endAt = request1.EndAt;

        var eventsQuery = new EventsQuery(null, null, null, 1, 10);

        //Act
        var isAddedOk = _eventService.TryAdd(request1);
        isAddedOk &= _eventService.TryAdd(request2);
        var paginatedEvents = _eventService.GetAll(eventsQuery);
        var eventsResponse = paginatedEvents.CurrentEvents;

        //Assert
        Assert.True(isAddedOk);

        foreach (var expected in new[] { request1, request2 })
        {
            var expectedId = Assert.Single(eventsResponse, e => e.Title == expected.Title).guid;
            var isEventFound = _eventService.TryGetById(expectedId, out var actualResponse);

            Assert.True(isEventFound);
            Assert.NotNull(actualResponse);
            Assert.Equal(expectedId, actualResponse.guid);
            Assert.Equal(expected.Title, actualResponse.Title);
            Assert.Equal(expected.StartAt, actualResponse.StartAt);
            Assert.Equal(expected.EndAt, actualResponse.EndAt);
        }
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

    private EventRequest GenerateNewEventRequest()
    {
        var randomTitle = _random.GetString(chars, 5);

        return GenerateNewEventRequest(randomTitle);
    }
}