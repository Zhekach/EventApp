namespace EventApp.Models;

public record EventResponse (Guid guid, string Title, DateTime StartAt, DateTime EndAt, string Description);