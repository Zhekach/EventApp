namespace EventApp.Models;

public record EventRequest(string Title, DateTime StartAt, DateTime EndAt, string Description);