namespace EventApp.Models;

public record EventsQuery(string? Title, DateTime? From, DateTime? To, 
    int Page, int PageSize);