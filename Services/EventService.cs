using EventEase.Models;

namespace EventEase.Services;

public class EventService
{
    private readonly List<Event> _events;
    private readonly Dictionary<int, Event> _byId;

    public EventService()
    {
        _events = new List<Event>
        {
            new Event { Id = 1, Name = "Tech Conference", Date = new DateTime(2026, 11, 15), Location = "Seattle, WA" },
            new Event { Id = 2, Name = "Design Meetup", Date = new DateTime(2026, 12, 3), Location = "Chicago, IL" },
            new Event { Id = 3, Name = "AI Summit", Date = new DateTime(2027, 1, 12), Location = "Boston, MA" }
        };

        var cities = new[] { "Seattle, WA", "Chicago, IL", "Boston, MA", "Austin, TX", "Denver, CO" };

        for (int i = 4; i <= 5000; i++)
        {
            _events.Add(new Event
            {
                Id = i,
                Name = $"Sample Event {i}",
                Date = DateTime.Today.AddDays(i % 365),
                Location = cities[i % cities.Length]
            });
        }

        _byId = _events.ToDictionary(e => e.Id);
    }

    public IReadOnlyList<Event> GetAll() => _events;

    public Event? GetById(int id) => _byId.TryGetValue(id, out var e) ? e : null;
}