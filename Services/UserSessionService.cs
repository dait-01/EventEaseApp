using EventEase.Models;

namespace EventEase.Services;

public class UserSessionService
{
    private readonly List<Registration> _registrations = new();

    public void AddRegistration(Registration registration)
    {
        _registrations.Add(registration);
    }

    public IEnumerable<Registration> GetRegistrationsForEvent(int eventId)
    {
        return _registrations.Where(r => r.EventId == eventId).ToList();
    }
}
