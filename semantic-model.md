**Semantic Model — Table Tennis Tracker**

- **Purpose**: concise reference of domain classes/tables, their main attributes, and relationships.

**Models**
- **TableTennisTracker.Domain.Models.Match**: [TableTennisTracker.Domain/Models/Match.cs](TableTennisTracker.Domain/Models/Match.cs#L1)
  - **Key attributes**: Id, TournamentId, RoundNumber, TableNumber, ScheduledStartUtc, ActualStartUtc?, CompletedUtc?, Status (MatchStatus), BestOfSets, WinnerPlayerId?
  - **Relations**: **Tournament** (many-to-one via `TournamentId`); optional **Player** (`WinnerPlayer` via `WinnerPlayerId`); one-to-many to **MatchParticipant** (`Participants`); one-to-many to **MatchSetResult** (`SetResults`).

- **TableTennisTracker.Domain.Models.Player**: [TableTennisTracker.Domain/Models/Player.cs](TableTennisTracker.Domain/Models/Player.cs#L1)
  - **Key attributes**: Id, FirstName, LastName, DateOfBirth, CountryCode, CurrentRankingPoints, IsActive, CreatedUtc
  - **Relations**: one-to-many **Registration** (`Registrations`); one-to-many **MatchParticipant** (`MatchParticipations`).

- **TableTennisTracker.Domain.Models.MatchParticipant**: [TableTennisTracker.Domain/Models/MatchParticipant.cs](TableTennisTracker.Domain/Models/MatchParticipant.cs#L1)
  - **Key attributes**: Id, MatchId, PlayerId, Slot (player position in match), ScoreSetsWon
  - **Relations**: belongs to **Match** (`MatchId`); belongs to **Player** (`PlayerId`).

- **TableTennisTracker.Domain.Models.MatchSetResult**: [TableTennisTracker.Domain/Models/MatchSetResult.cs](TableTennisTracker.Domain/Models/MatchSetResult.cs#L1)
  - **Key attributes**: Id, MatchId, SetNumber, PlayerOnePoints, PlayerTwoPoints
  - **Relations**: belongs to **Match** (`MatchId`).

- **TableTennisTracker.Domain.Models.Registration**: [TableTennisTracker.Domain/Models/Registration.cs](TableTennisTracker.Domain/Models/Registration.cs#L1)
  - **Key attributes**: Id, PlayerId, TournamentId, RegisteredUtc, SeedNumber, IsCheckedIn
  - **Relations**: belongs to **Player** (`PlayerId`); belongs to **Tournament** (`TournamentId`).

- **TableTennisTracker.Domain.Models.Tournament**: [TableTennisTracker.Domain/Models/Tournament.cs](TableTennisTracker.Domain/Models/Tournament.cs#L1)
  - **Key attributes**: Id, Name, SeasonLabel, StartUtc, EndUtc, MaxPlayers, BestOfSets, OrganizerName, IsRankingEvent, VenueId
  - **Relations**: belongs to **Venue** (`VenueId`); one-to-many **Registration** (`Registrations`); one-to-many **Match** (`Matches`).

- **TableTennisTracker.Domain.Models.Venue**: [TableTennisTracker.Domain/Models/Venue.cs](TableTennisTracker.Domain/Models/Venue.cs#L1)
  - **Key attributes**: Id, Name, City, CountryCode, AddressLine, NumberOfTables, Capacity, TimeZoneId
  - **Relations**: one-to-many **Tournament** (`Tournaments`).

**Cross-model relationships (summary)**
- Tournament 1..* Matches (Tournament.Matches)
- Tournament 1..* Registrations (Tournament.Registrations)
- Venue 1..* Tournaments (Venue.Tournaments)
- Registration  *1* -> 1 Player (Registration.Player) and 1 Tournament (Registration.Tournament)
- Match 1..* MatchParticipant (Match.Participants) — each participant links to a `Player`.
- Match 1..* MatchSetResult (Match.SetResults) — individual set scores.
- Player 1..* MatchParticipant (Player.MatchParticipations) — players may appear in many matches.
- Optional Match.WinnerPlayer references Player (nullable).

**Enums & supporting types**
- `MatchStatus` enum lives under [TableTennisTracker.Domain/Enums/MatchStatus.cs](TableTennisTracker.Domain/Enums/MatchStatus.cs#L1) and is used by `Match.Status` to represent state (Scheduled/InProgress/Completed/etc.).

**Notes / Design considerations**
- Ids are GUIDs across entities to simplify distributed uniqueness and linking.
- Several nullable foreign keys (e.g., `WinnerPlayerId`) represent optional relations.
- Collections are modeled with virtual `ICollection<T>` to support EF Core lazy/proxy behavior where applicable.
- Date/time fields use UTC (`StartUtc`, `ScheduledStartUtc`, `CreatedUtc`) — time zone handling is stored on `Venue.TimeZoneId` for local scheduling logic.

If you'd like, I can also produce a Mermaid ER diagram file or add this summary to the project README. 
