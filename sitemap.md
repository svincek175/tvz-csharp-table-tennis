**Sitemap — Table Tennis Tracker**

- **Purpose**: maps app URLs to controller actions and the view or response type used.

**Global / root**
- `/` (GET): `HomeController.Index()` → View: [TableTennisTracker.Web/Views/Home/Index.cshtml](TableTennisTracker.Web/Views/Home/Index.cshtml)
- `/Home` (GET): `HomeController.Index()` → View: [TableTennisTracker.Web/Views/Home/Index.cshtml](TableTennisTracker.Web/Views/Home/Index.cshtml)
- `/Home/Index` (GET): same as above

**Matches (custom routes applied)**
- `/matches/all` (GET): `MatchesController.Index()` → View: [TableTennisTracker.Web/Views/Matches/Index.cshtml](TableTennisTracker.Web/Views/Matches/Index.cshtml)
- `/matches/view/{id:guid}` (GET): `MatchesController.Details(Guid id)` → View: [TableTennisTracker.Web/Views/Matches/Details.cshtml](TableTennisTracker.Web/Views/Matches/Details.cshtml)
- NOTE: conventional `/Matches` and `/Matches/Details/{id}` may not be the intended public routes — the app defines the explicit attribute routes above.

**Players (custom routes applied)**
- `/players/list` (GET): `PlayersController.Index()` → View: [TableTennisTracker.Web/Views/Players/Index.cshtml](TableTennisTracker.Web/Views/Players/Index.cshtml)
- `/players/profile/{id:guid}` (GET): `PlayersController.Details(Guid id)` → View: [TableTennisTracker.Web/Views/Players/Details.cshtml](TableTennisTracker.Web/Views/Players/Details.cshtml)

**Registrations (custom routes applied)**
- `/registrations/current` (GET): `RegistrationsController.Index()` → View: [TableTennisTracker.Web/Views/Registrations/Index.cshtml](TableTennisTracker.Web/Views/Registrations/Index.cshtml)
- `/registrations/info/{id:guid}` (GET): `RegistrationsController.Details(Guid id)` → View: [TableTennisTracker.Web/Views/Registrations/Details.cshtml](TableTennisTracker.Web/Views/Registrations/Details.cshtml)

**Venues (custom routes applied)**
- `/venues/overview` (GET): `VenuesController.Index()` → View: [TableTennisTracker.Web/Views/Venues/Index.cshtml](TableTennisTracker.Web/Views/Venues/Index.cshtml)
- `/venues/location/{id:guid}` (GET): `VenuesController.Details(Guid id)` → View: [TableTennisTracker.Web/Views/Venues/Details.cshtml](TableTennisTracker.Web/Views/Venues/Details.cshtml)

**Tournaments (conventional routes)**
- `/Tournaments` or `/Tournaments/Index` (GET): `TournamentsController.Index()` → View: [TableTennisTracker.Web/Views/Tournaments/Index.cshtml](TableTennisTracker.Web/Views/Tournaments/Index.cshtml)
- `/Tournaments/Details/{id:guid}` (GET): `TournamentsController.Details(Guid id)` → View: [TableTennisTracker.Web/Views/Tournaments/Details.cshtml](TableTennisTracker.Web/Views/Tournaments/Details.cshtml)

**MatchParticipants (conventional routes)**
- `/MatchParticipants` or `/MatchParticipants/Index` (GET): `MatchParticipantsController.Index()` → View: [TableTennisTracker.Web/Views/MatchParticipants/Index.cshtml](TableTennisTracker.Web/Views/MatchParticipants/Index.cshtml)
- `/MatchParticipants/Details/{id:guid}` (GET): `MatchParticipantsController.Details(Guid id)` → View: [TableTennisTracker.Web/Views/MatchParticipants/Details.cshtml](TableTennisTracker.Web/Views/MatchParticipants/Details.cshtml)

**MatchSetResults (conventional routes)**
- `/MatchSetResults` or `/MatchSetResults/Index` (GET): `MatchSetResultsController.Index()` → View: [TableTennisTracker.Web/Views/MatchSetResults/Index.cshtml](TableTennisTracker.Web/Views/MatchSetResults/Index.cshtml)
- `/MatchSetResults/Details/{id:guid}` (GET): `MatchSetResultsController.Details(Guid id)` → View: [TableTennisTracker.Web/Views/MatchSetResults/Details.cshtml](TableTennisTracker.Web/Views/MatchSetResults/Details.cshtml)

**Quiz**
- `/Quiz` or `/Quiz/Index` (GET): `QuizController.Index()` → View: [TableTennisTracker.Web/Views/Quiz/Index.cshtml](TableTennisTracker.Web/Views/Quiz/Index.cshtml)
- `/Quiz/GetQuestions` (GET): `QuizController.GetQuestions()` → JSON list of `QuizQuestion` objects (API endpoint)
- `/Quiz/SubmitAnswer` (POST): `QuizController.SubmitAnswer([FromBody] AnswerSubmission)` → JSON result `{ isCorrect: bool }`

**Static/shared layout**
- Views use shared layout: [TableTennisTracker.Web/Views/Shared/_Layout.cshtml](TableTennisTracker.Web/Views/Shared/_Layout.cshtml)

**Routing notes**
- The app uses conventional controller/action routing for controllers without attribute routes; for several controllers we added explicit attribute routes (`Matches`, `Players`, `Registrations`, `Venues`) — those are the canonical public endpoints for those resources.
- All Details routes expect a GUID parameter named `id` (constraint `{id:guid}` used in attribute templates where applied).
- API-like endpoints (Quiz) return JSON and are usable from JS or external clients.

If you'd like, I can also generate a machine-readable JSON sitemap or a Mermaid site diagram next. 
