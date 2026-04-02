using TableTennisTracker.Domain.Enums;
using TableTennisTracker.Domain.Models;

var now = new DateTime(2026, 04, 02, 09, 00, 0, DateTimeKind.Utc);

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

venues[0].Tournaments.Add(tournaments[0]);
venues[1].Tournaments.Add(tournaments[1]);
venues[2].Tournaments.Add(tournaments[2]);

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

var tournamentPlayers = new Dictionary<Guid, List<Player>>
{
    [tournaments[0].Id] = players.GetRange(0, 3),
    [tournaments[1].Id] = players.GetRange(3, 3),
    [tournaments[2].Id] = players.GetRange(6, 3)
};

var registrations = new List<Registration>();
var matches = new List<Match>();
var participants = new List<MatchParticipant>();
var setResults = new List<MatchSetResult>();

for (var tournamentIndex = 0; tournamentIndex < tournaments.Count; tournamentIndex++)
{
    var tournament = tournaments[tournamentIndex];
    var playersForTournament = tournamentPlayers[tournament.Id];

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

    foreach (var registration in tournamentRegistrations)
    {
        registration.Player!.Registrations.Add(registration);
    }

    var tournamentMatches = new List<Match>();

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

        tournamentMatches.Add(match);
        participants.AddRange(matchParticipants);
        setResults.AddRange(matchSetResults);
    }

    tournament.Matches = tournamentMatches;
    matches.AddRange(tournamentMatches);
}

foreach (var player in players)
{
    player.MatchParticipations = participants.Where(p => p.PlayerId == player.Id).ToList();
}

Console.WriteLine("🏆 Tournament root model initialized with 3 tournament instances");
foreach (var tournament in tournaments)
{
    Console.WriteLine($"\n• {tournament.Name} ({tournament.SeasonLabel})");
    Console.WriteLine($"  Venue: {tournament.Venue?.Name}, {tournament.Venue?.City}");
    Console.WriteLine($"  Registrations: {tournament.Registrations.Count}");
    Console.WriteLine($"  Matches: {tournament.Matches.Count}");
    Console.WriteLine($"  Match Participants: {tournament.Matches.Sum(m => m.Participants.Count)}");
    Console.WriteLine($"  Set Results: {tournament.Matches.Sum(m => m.SetResults.Count)}");
}

Console.WriteLine("\n🔍 LINQ QUERIES FOR APP USE-CASES\n");

Console.WriteLine("1) Upcoming tournaments (date ordered):");
var upcomingTournaments = tournaments
    .Where(t => t.StartUtc >= now.Date)
    .OrderBy(t => t.StartUtc)
    .Select(t => new { t.Name, t.StartUtc, Venue = t.Venue!.Name })
    .ToList();
foreach (var item in upcomingTournaments)
{
    Console.WriteLine($"   {item.StartUtc:yyyy-MM-dd}: {item.Name} @ {item.Venue}");
}

Console.WriteLine("\n2) Check-in progress by tournament:");
var checkInProgress = tournaments
    .Select(t => new
    {
        t.Name,
        Total = t.Registrations.Count,
        CheckedIn = t.Registrations.Count(r => r.IsCheckedIn)
    })
    .ToList();
foreach (var item in checkInProgress)
{
    Console.WriteLine($"   {item.Name}: {item.CheckedIn}/{item.Total} checked in");
}

Console.WriteLine("\n3) Match status counts across all tournaments:");
var matchStatusCounts = matches
    .GroupBy(m => m.Status)
    .Select(group => new { Status = group.Key, Count = group.Count() })
    .OrderBy(x => x.Status)
    .ToList();
foreach (var item in matchStatusCounts)
{
    Console.WriteLine($"   {item.Status}: {item.Count}");
}

Console.WriteLine("\n4) Top players by win count:");
var topWinners = matches
    .Where(m => m.Status == MatchStatus.Completed && m.WinnerPlayer is not null)
    .GroupBy(m => m.WinnerPlayer!)
    .Select(group => new
    {
        PlayerName = $"{group.Key.FirstName} {group.Key.LastName}",
        Wins = group.Count()
    })
    .OrderByDescending(x => x.Wins)
    .ThenBy(x => x.PlayerName)
    .ToList();
foreach (var item in topWinners)
{
    Console.WriteLine($"   {item.PlayerName}: {item.Wins} win(s)");
}

Console.WriteLine("\n5) Per-tournament average sets per match:");
var averageSetsPerTournament = tournaments
    .Select(t => new
    {
        t.Name,
        AverageSets = t.Matches.Any() ? t.Matches.Average(m => m.SetResults.Count) : 0
    })
    .ToList();
foreach (var item in averageSetsPerTournament)
{
    Console.WriteLine($"   {item.Name}: {item.AverageSets:F1} sets/match");
}

Console.WriteLine("\n6) Top seed per tournament:");
var topSeedPerTournament = tournaments
    .Select(t => new
    {
        t.Name,
        TopSeed = t.Registrations
            .OrderBy(r => r.SeedNumber)
            .Select(r => new { r.SeedNumber, PlayerName = $"{r.Player!.FirstName} {r.Player.LastName}" })
            .FirstOrDefault()
    })
    .ToList();
foreach (var item in topSeedPerTournament)
{
    Console.WriteLine($"   {item.Name}: Seed #{item.TopSeed?.SeedNumber} - {item.TopSeed?.PlayerName}");
}

Console.WriteLine("\n✅ Refactor complete: tournament is the root object with 3 fully branched instances.");
