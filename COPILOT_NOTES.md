# Copilot Usage Summary — EventEase

This document describes how GitHub Copilot was used across the three project
activities, including the prompts given, what Copilot generated, and the
corrections that were needed at each step.

## Activity 1 — Data Binding, Components, and Routing

**Event Card and model.** Prompted Copilot to create an `EventCard` component
with two-way data binding to an `Event` model (Name, Date, Location). Copilot
generated the component using the `[Parameter] Event` /
`[Parameter] EventChanged` pairing, which is the naming convention Blazor
requires for `@bind-Event` to work from a parent. This let the card support
real two-way binding rather than a simpler read-only display.

**Event list and routing.** Prompted Copilot to build an event list page at
`/events` and a details page at `/events/{id}`, using `NavLink` to connect
them. Copilot's first version of the list omitted an `Id` property on the
`Event` model, which had to be added manually before the routing code would
compile — a reminder that generated code across multiple files can have
mismatched assumptions about a shared model.

**Registration page.** Prompted Copilot to scaffold a registration route
tied to the selected event. Copilot correctly looked up the event by ID and
displayed its name, matching the pattern already established in the details
page.

**Known limitation introduced here:** the event data was duplicated as a
hardcoded list in three separate files (list, details, registration). This
was accepted for Activity 1 and resolved in Activity 2.

## Activity 2 — Debugging and Optimization

**Input validation.** Prompted Copilot to add `DataAnnotations`-based
validation (required fields, string length, valid date) to the `Event`
model and wire it into `EditForm` / `DataAnnotationsValidator` /
`ValidationMessage` in the Event Card. Copilot made an important adjustment
on its own: it changed `Date` from `DateTime` to `DateTime?`. This was
necessary — a non-nullable `DateTime` can never be empty, so `[Required]`
on it can never fire. Making it nullable let the validation actually work
as intended.

**Shared event service.** Prompted Copilot to create an `EventService`
with `GetAll()` and `GetById()`, registered as a singleton, and to update
all three pages to use it instead of their own hardcoded lists. This
resolved the data-duplication issue from Activity 1. Copilot also added a
dictionary-based lookup inside the service for faster `GetById` performance
once the dataset grew large in the next step.

**Routing error handling.** Prompted Copilot to build a reusable
"not found" component and use it wherever an invalid event ID was
requested, plus a friendlier fallback page for entirely invalid routes.

**A real bug caught during testing:** after wiring the not-found component
into the Registration page, `/register/99` rendered as a blank page instead
of showing the error. The cause was a missing `@using` directive for the
component's namespace in that specific file — Blazor silently renders an
unrecognized tag as nothing rather than throwing a runtime error, which
made the bug easy to miss without deliberately testing that exact route.
The fix was adding the missing `@using`. This was a useful reminder that a
component generated and working correctly in one file still needs its
namespace explicitly imported in every other file that uses it.

**Performance with large datasets.** Prompted Copilot to expand the event
service to 5,000 sample events (to create a realistic bottleneck) and to
optimize the event list using Blazor's `Virtualize` component. Before the
fix, the browser was rendering roughly 5,000 full event cards at once,
confirmed by checking `document.querySelectorAll('.event-card').length` in
DevTools. After adding `Virtualize`, that count dropped to roughly 10–15
rendered cards at any time, with the rest rendered on demand as the user
scrolls. This was the clearest before/after evidence of a real performance
fix in the project.

## Activity 3 — Advanced Features

**Registration form with validation.** Prompted Copilot to build a proper
`Registration` model (Name, Email with `[EmailAddress]`) and wire it into
an `EditForm` with `OnValidSubmit`, replacing the two placeholder inputs
from Activity 1. Copilot correctly gated the confirmation message on
successful validation.

**Session state.** Prompted Copilot to create a `UserSessionService` that
stores registrations in memory and can filter them by event. Registered as
scoped, consistent with the idea that registration data belongs to the
current session rather than being shared globally like the event catalog.

**Attendance Tracker.** Prompted Copilot to build a page that displays all
registrations for a given event using the session service. Testing this
page — registering for two different events and confirming each event's
tracker only shows its own registrants — was the practical proof that the
session state was scoped correctly per event rather than leaking across
them.

**Consistency check caught by review, not by Copilot:** the first version
of the Attendance Tracker used a plain "Event not found" paragraph instead
of the reusable not-found component used everywhere else in the app. Since
Copilot only sees the file it's currently working in, it didn't
automatically match a pattern established in a different file. This was
caught by comparing the new page against the established convention and
fixed manually.

## Production Cleanup

Prompted Copilot to review the full project for unused `using` directives,
dead code, and leftover debug statements. It correctly reported no
remaining `Console.WriteLine` calls and no dead code branches, and flagged
the unused default `HttpClient` registration in `Program.cs` as harmless
scaffolding left over from the project template (since EventEase makes no
external API calls). One finding from this review — a supposedly unused
`@using EventEase.Components` in the Attendance Tracker — turned out to be
incorrect on manual inspection, since that page's not-found fix still
depended on it. This was a useful reminder to verify AI-generated review
findings against the actual file contents rather than applying them
automatically, since removing that line would have broken the page.

## Overall Takeaways

- Copilot was fastest and most reliable for scaffolding well-defined,
  self-contained pieces: a model class, a component with named parameters,
  a service with clearly described methods.
- Copilot's suggestions were most likely to need correction at the
  boundaries between files — missing properties a related file assumed
  existed (`Id`), missing imports for components used elsewhere
  (`@using`), and inconsistent patterns across pages it wasn't shown
  together (the not-found handling).
- Testing each feature against specific edge cases (empty fields, invalid
  IDs, non-numeric routes, large datasets) surfaced real bugs that were not
  obvious from reading the generated code alone.
- Treating Copilot's own code-review output as a suggestion to verify,
  rather than an instruction to apply directly, prevented at least one
  incorrect change from being made during cleanup.