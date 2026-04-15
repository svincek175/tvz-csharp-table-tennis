using TableTennisTracker.Domain.Enums;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Infrastructure;

public static class DataSeeder
{
    public static (List<Venue>, List<Tournament>, List<Player>, List<Registration>, List<Match>, List<MatchParticipant>, List<MatchSetResult>) SeedData()
    {
        var now = new DateTime(2026, 04, 15, 09, 00, 0, DateTimeKind.Utc);

        var venues = new List<Venue>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Zagreb Sports Hall",
                City = "Zagreb",
                CountryCode = "HR",
                AddressLine = "Av. Većeslava Holjevca 10",
                NumberOfTables = 12,
                Capacity = 500,
                TimeZoneId = "Europe/Zagreb"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Split Arena Center",
                City = "Split",
                CountryCode = "HR",
                AddressLine = "Zrinsko-Frankopanska 12",
                NumberOfTables = 10,
                Capacity = 420,
                TimeZoneId = "Europe/Zagreb"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Rijeka Indoor Arena",
                City = "Rijeka",
                CountryCode = "HR",
                AddressLine = "Korzo 21",
                NumberOfTables = 8,
                Capacity = 350,
                TimeZoneId = "Europe/Zagreb"
            }
        };

        var tournaments = new List<Tournament>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Croatian Open 2026",
                SeasonLabel = "Spring 2026",
                StartUtc = new DateTime(2026, 04, 15, 08, 00, 0, DateTimeKind.Utc),
                EndUtc = new DateTime(2026, 04, 17, 18, 00, 0, DateTimeKind.Utc),
                MaxPlayers = 64,
                BestOfSets = 5,
                OrganizerName = "Croatian Table Tennis Association",
                IsRankingEvent = true,
                VenueId = venues[0].Id,
                Venue = venues[0]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Dalmatia Masters 2026",
                SeasonLabel = "Summer 2026",
                StartUtc = new DateTime(2026, 06, 10, 08, 00, 0, DateTimeKind.Utc),
                EndUtc = new DateTime(2026, 06, 12, 18, 00, 0, DateTimeKind.Utc),
                MaxPlayers = 48,
                BestOfSets = 5,
                OrganizerName = "Split Regional Federation",
                IsRankingEvent = true,
                VenueId = venues[1].Id,
                Venue = venues[1]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Adriatic Cup 2026",
                SeasonLabel = "Autumn 2026",
                StartUtc = new DateTime(2026, 09, 03, 08, 00, 0, DateTimeKind.Utc),
                EndUtc = new DateTime(2026, 09, 05, 18, 00, 0, DateTimeKind.Utc),
                MaxPlayers = 32,
                BestOfSets = 5,
                OrganizerName = "Adriatic Sports League",
                IsRankingEvent = false,
                VenueId = venues[2].Id,
                Venue = venues[2]
            }
        };

        var players = new List<Player>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Marko", LastName = "Horvat", DateOfBirth = new DateOnly(1995, 05, 12), CountryCode = "HR", CurrentRankingPoints = 1250, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Ana", LastName = "Kovačević", DateOfBirth = new DateOnly(1998, 03, 28), CountryCode = "HR", CurrentRankingPoints = 980, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Igor", LastName = "Petrović", DateOfBirth = new DateOnly(1992, 07, 15), CountryCode = "RS", CurrentRankingPoints = 1150, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Luka", LastName = "Barišić", DateOfBirth = new DateOnly(1997, 01, 06), CountryCode = "HR", CurrentRankingPoints = 1010, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Petra", LastName = "Vuković", DateOfBirth = new DateOnly(2001, 09, 02), CountryCode = "HR", CurrentRankingPoints = 920, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Nina", LastName = "Jurić", DateOfBirth = new DateOnly(1999, 04, 21), CountryCode = "BA", CurrentRankingPoints = 970, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Filip", LastName = "Rukavina", DateOfBirth = new DateOnly(1994, 12, 11), CountryCode = "HR", CurrentRankingPoints = 1075, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Sara", LastName = "Matić", DateOfBirth = new DateOnly(2002, 08, 13), CountryCode = "SI", CurrentRankingPoints = 860, IsActive = true, CreatedUtc = now },
            new() { Id = Guid.NewGuid(), FirstName = "Dino", LastName = "Perković", DateOfBirth = new DateOnly(1996, 02, 19), CountryCode = "ME", CurrentRankingPoints = 990, IsActive = true, CreatedUtc = now }
        };

        var registrations = new List<Registration>();
        var matches = new List<Match>();
        var participants = new List<MatchParticipant>();
        var setResults = new List<MatchSetResult>();

        for (var tournamentIndex = 0; tournamentIndex < tournaments.Count; tournamentIndex++)
        {
            var tournament = tournaments[tournamentIndex];
            var playersForTournament = new List<Player> { players[tournamentIndex * 3], players[tournamentIndex * 3 + 1], players[tournamentIndex * 3 + 2] };

            var tournamentRegistrations = playersForTournament
                .Select((player, index) => new Registration
                {
                    Id = Guid.NewGuid(),
                    PlayerId = player.Id,
                    TournamentId = tournament.Id,
                    RegisteredUtc = tournament.StartUtc.AddDays(-15 + index),
                    SeedNumber = index + 1,
                    IsCheckedIn = index != 2,
                    Player = player,
                    Tournament = tournament
                })
                .ToList();

            tournament.Registrations = tournamentRegistrations;
            registrations.AddRange(tournamentRegistrations);

            for (var matchIndex = 0; matchIndex < 3; matchIndex++)
            {
                var firstPlayer = playersForTournament[matchIndex % 3];
                var secondPlayer = playersForTournament[(matchIndex + 1) % 3];

                var status = matchIndex switch
                {
                    0 => MatchStatus.Completed,
                    1 => MatchStatus.InProgress,
                    _ => MatchStatus.Scheduled
                };

                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournament.Id,
                    RoundNumber = 1,
                    TableNumber = 1 + matchIndex,
                    ScheduledStartUtc = tournament.StartUtc.AddHours(1 + (matchIndex * 2)),
                    ActualStartUtc = status is MatchStatus.Completed or MatchStatus.InProgress
                        ? tournament.StartUtc.AddHours(1 + (matchIndex * 2)).AddMinutes(5)
                        : null,
                    CompletedUtc = status == MatchStatus.Completed
                        ? tournament.StartUtc.AddHours(2 + (matchIndex * 2))
                        : null,
                    Status = status,
                    BestOfSets = tournament.BestOfSets,
                    WinnerPlayerId = status == MatchStatus.Completed ? firstPlayer.Id : null,
                    Tournament = tournament,
                    WinnerPlayer = status == MatchStatus.Completed ? firstPlayer : null
                };

                var matchParticipants = new List<MatchParticipant>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        MatchId = match.Id,
                        PlayerId = firstPlayer.Id,
                        Slot = 1,
                        ScoreSetsWon = status == MatchStatus.Completed ? 3 : 1,
                        Match = match,
                        Player = firstPlayer
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        MatchId = match.Id,
                        PlayerId = secondPlayer.Id,
                        Slot = 2,
                        ScoreSetsWon = status == MatchStatus.Completed ? 1 : 1,
                        Match = match,
                        Player = secondPlayer
                    }
                };

                var matchSetResults = new List<MatchSetResult>
                {
                    new() { Id = Guid.NewGuid(), MatchId = match.Id, SetNumber = 1, PlayerOnePoints = 11, PlayerTwoPoints = 9, Match = match },
                    new() { Id = Guid.NewGuid(), MatchId = match.Id, SetNumber = 2, PlayerOnePoints = 8, PlayerTwoPoints = 11, Match = match },
                    new() { Id = Guid.NewGuid(), MatchId = match.Id, SetNumber = 3, PlayerOnePoints = 11, PlayerTwoPoints = 7, Match = match }
                };

                match.Participants = matchParticipants;
                match.SetResults = matchSetResults;

                matches.Add(match);
                participants.AddRange(matchParticipants);
                setResults.AddRange(matchSetResults);
            }

            tournament.Matches = tournaments.Where(t => t.Id == tournament.Id).First().Matches = matches.Where(m => m.TournamentId == tournament.Id).ToList();
        }

        return (venues, tournaments, players, registrations, matches, participants, setResults);
    }
}
