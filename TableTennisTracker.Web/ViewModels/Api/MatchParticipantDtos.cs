using System;

namespace TableTennisTracker.Web.ViewModels.Api
{
    public record MatchParticipantPlayerBriefDto(Guid Id, string FirstName, string LastName);

    public record MatchParticipantDto(Guid Id, Guid MatchId, Guid PlayerId, int Slot, int ScoreSetsWon, MatchParticipantPlayerBriefDto? Player = null);

    public record MatchParticipantCreateDto(Guid MatchId, Guid PlayerId, int Slot, int ScoreSetsWon);

    public record MatchParticipantUpdateDto(Guid MatchId, Guid PlayerId, int Slot, int ScoreSetsWon);
}
