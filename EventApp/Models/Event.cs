namespace EventApp.Models;

public record Event 
{
    public Guid Id { get; }
    public string Title { get; private init; }
    public DateTime StartAt { get; private init; }
    public DateTime EndAt { get; private init; }
    public string Description { get; private init; }

    private Event(string title, DateTime startAt, DateTime endAt, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        StartAt = startAt;
        EndAt = endAt;
        Description = description;
    }

    public static Event Create(string title, DateTime startAt,  DateTime endAt, string description = "")
    {
        ValidateTitle(title);
        Event result = new Event(title, startAt, endAt, description);
        
        return result;
    }

    public Event Update(Event newEvent)
    {
        return this with
        {
            Description = newEvent.Description,
            StartAt = newEvent.StartAt,
            EndAt = newEvent.EndAt
        };
    }
    
    private static void ValidateTitle(string title)
    {
        if(string.IsNullOrWhiteSpace(title))
            throw  new ArgumentNullException(nameof(title), "Название не может быть пустым");
    }
}