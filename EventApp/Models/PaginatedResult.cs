namespace EventApp.Models;

public record PaginatedResult(int TotalEventsCount, List<EventResponse> CurrentEvents, 
    int CurrentPageNumber, int CurrentPageEventsCount);