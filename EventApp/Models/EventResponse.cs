namespace EventApp.Models;

public record EventResponse (Guid Guid, string Title, DateTime StartAt, DateTime EndAt, string Description);