using System;

namespace TableTennisTracker.Web.ViewModels.Api
{
    public record TournamentVenueBriefDto(Guid Id, string Name, string City);

    public record TournamentDto(Guid Id, string Name, string SeasonLabel, DateTime StartUtc, DateTime EndUtc, int MaxPlayers, int BestOfSets, string OrganizerName, bool IsRankingEvent, Guid VenueId, TournamentVenueBriefDto? Venue = null);

    public record TournamentCreateDto(string Name, string SeasonLabel, DateTime StartUtc, DateTime EndUtc, int MaxPlayers, int BestOfSets, string OrganizerName, bool IsRankingEvent, Guid VenueId);

    public record TournamentUpdateDto(string Name, string SeasonLabel, DateTime StartUtc, DateTime EndUtc, int MaxPlayers, int BestOfSets, string OrganizerName, bool IsRankingEvent, Guid VenueId);
}
